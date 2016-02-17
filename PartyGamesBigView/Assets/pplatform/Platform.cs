using UnityEngine;
using System.Collections;
using System;
using System.Text;

using System.Collections.Generic;
using PPlatform.Helper;
using UnityEngine.SceneManagement;

namespace PPlatform
{
    public class Platform : UnitySingleton<Platform>
    {
        public static readonly int VIEW_USER_ID = 0;
        public static readonly int HOST_CONTROLLER_USER_ID = 1;

        private int mNextUserId = HOST_CONTROLLER_USER_ID; //start with the host controller user id


        public KeyCode _ExitKey = KeyCode.Escape;

        private ANetgroup mNetgroup = null;
        private string mActiveName = "gamelist";
        private Dictionary<int, Controller> mController = new Dictionary<int, Controller>();
        public Dictionary<int, int> mConnectionIdToUserId = new Dictionary<int, int>();
        public Dictionary<int, int> mUserIdToConnectionId = new Dictionary<int, int>();

        private string mGameCode;

        public string GameCode
        {
            get { return mGameCode; }
        }

        public Dictionary<int, Controller> Controllers
        {
            get { return mController; }
        }


        string TAG_CONTROLLER_REGISTER = "PLATFORM_CONTROLLER_REGISTER";
        struct ControllerRegisterMessage
        {
            public string name;
        }

        string TAG_CONTROLLER_DISCOVERY = "PLATFORM_CONTROLLER_DISCOVERY";
        private struct ControllerDiscoveryMessage
        {
            public int connectionId;
            public int userId;
            public string name;
        }

        string TAG_CONTROLLER_LEFT = "PLATFORM_CONTROLLER_LEFT";
        //no content


        string TAG_VIEW_DISCOVERY = "PLATFORM_VIEW_DISCOVERY";
        //no content

        string TAG_ENTER_GAME = "PLATFORM_ENTER_GAME";
        //string content

        string TAG_EXIT_GAME = "PLATFORM_EXIT_GAME";


        private int mOwnId = -1;
        public event Action<string, string, int> Message;

        private struct PlatformMessage
        {
            public string tag;
            public string content;
        }

        private void Awake()
        {
            mNetgroup = GetComponent<UnityNetgroup>();
            if(mNetgroup == null)
            {
                mNetgroup = this.gameObject.AddComponent<UnityNetgroup>();
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            mGameCode = GetRandomKey();
            mNetgroup.Open(mGameCode, OnNetgroupMessageInternal);
        }

        private void OnGUI()
        {
            //GUILayout.BeginVertical();
            //if (GUILayout.Button("Exit"))
            //{
            //    mNetgroup.Close();
            //}
            //GUILayout.EndHorizontal();
        }

        public void EnterGame(string name)
        {
            Send(TAG_ENTER_GAME, name, -1);
        }
        private void ShowGame(string name)
        {
            Debug.Log("Show game " + name);
            SceneManager.LoadScene(name);
            //mActiveName = name;
        }

        /// <summary>
        /// Tells the platform that a new game was loaded
        /// </summary>
        /// <param name="name"></param>
        public void GameLoaded(string name)
        {
            mActiveName = name;
        }
        private void HandlePlatformMessage(string tag, string content, int userId)
        {


            //send the message out to the games
            if (Message != null)
                Message(tag, content, userId);

            if (tag == TAG_ENTER_GAME)
            {
                ShowGame(content);
            }

            if (tag == TAG_CONTROLLER_LEFT)
            {
                Debug.Log("Remove " + mController[userId]);
                //remove controller
                RemoveController(userId);
            }

        }

        private void AddController(int connectionId, int userId, string name)
        {
            Controller c = new Controller(userId, name);
            Debug.Log("Added new " + c);
            mController.Add(userId, c);
            mConnectionIdToUserId[connectionId] = userId;
            mUserIdToConnectionId[userId] = userId;
        }
        public void RemoveController(int userId)
        {
            int conId = mUserIdToConnectionId[userId];
            mController.Remove(userId);
            mUserIdToConnectionId.Remove(conId);
            mConnectionIdToUserId.Remove(conId);
        }
        public int ConnectionIdToUserId(int connectionId)
        {
            if (connectionId == -1)
                return -1;
            return connectionId;
        }

        public int UserIdToConnectionId(int userId)
        {
            if (userId == -1)
                return -1;
            return userId;
        }
        private void OnNetgroupMessageInternal(ANetgroup.SignalingMessageType type, int conId, string content)
        {
            //Debug.Log("Message type: " + type + " con id: " + conId + " content: " + content);
            if (type == ANetgroup.SignalingMessageType.Connected)
            {
                mOwnId = conId;
                mGameCode = content;
            }
            else if (type == ANetgroup.SignalingMessageType.UserJoined)
            {
                //send out the view discovery message to the new user so the controller knows how to contact the view
                Send(TAG_VIEW_DISCOVERY, "", conId);
            }
            else if (type == ANetgroup.SignalingMessageType.UserLeft)
            {
                HandlePlatformMessage(TAG_CONTROLLER_LEFT, null, conId);
            }
            else if (type == ANetgroup.SignalingMessageType.UserMessage)
            {
                PlatformMessage pm = JsonWrapper.FromJson<PlatformMessage>(content);

                //special platform messages which are handled before controllers are fully registerd
                if (pm.tag == TAG_CONTROLLER_REGISTER)
                {
                    //send out discovery broadcast (which will be received locally too and handled below)

                    ControllerRegisterMessage msg = JsonWrapper.FromJson<ControllerRegisterMessage>(pm.content);
                    Debug.Log("ControllerRegisterMessage received");
                    ControllerDiscoveryMessage discoveryMsg = new ControllerDiscoveryMessage();
                    discoveryMsg.connectionId = conId;
                    discoveryMsg.userId = conId;
                    discoveryMsg.name = msg.name;

                    PlatformMessage pmout = new PlatformMessage();
                    pmout.tag = TAG_CONTROLLER_DISCOVERY;
                    pmout.content = JsonWrapper.ToJson(discoveryMsg);

                    //use internal as the user isn't discovered locally yet before this message is received
                    SendInternal(JsonWrapper.ToJson(pmout));

                    //DO NOT PASS THIS MESSAGE TO HandlePlatformMessage. this message is internal only
                    //wait until controller discovery is received.
                }else if (pm.tag == TAG_CONTROLLER_DISCOVERY)
                {
                    //add controller to the list
                    ControllerDiscoveryMessage discoveryMsg = JsonWrapper.FromJson<ControllerDiscoveryMessage>(pm.content);
                    AddController(discoveryMsg.connectionId, discoveryMsg.userId, discoveryMsg.name);


                    Send(TAG_ENTER_GAME, mActiveName, discoveryMsg.userId);

                    //controller is known now -> handle the messages
                    HandlePlatformMessage(pm.tag, pm.content, discoveryMsg.userId);
                }
                else
                {
                    HandlePlatformMessage(pm.tag, pm.content, ConnectionIdToUserId(conId));
                }

            }




        }


        /// <summary>
        /// Sends a message to a certain user id or to everyone (including back to the sender)
        /// 
        /// 
        /// </summary>
        /// <param name="tag"> Any kind of tag. Make sure it is unique to avoid conflicts with other games</param>
        /// <param name="content">string, ideally json as content</param>
        /// <param name="lToUserId">A user id or -1 for broadcast</param>
        public void Send(string tag, string content, int lToUserId = -1)
        {
            PlatformMessage pm = new PlatformMessage();
            pm.tag = tag;
            pm.content = content;

            int connectionId = UserIdToConnectionId(lToUserId);
            SendInternal(JsonWrapper.ToJson(pm), connectionId);
        }
        
        /// <summary>
        /// Internal message: Sending a string directly to the message system via the connection id.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="lToConnectionId"></param>
        private void SendInternal(string content, int lToConnectionId = -1)
        {
            mNetgroup.SendMessageTo(content, lToConnectionId);
        }

        private string GetRandomKey()
        {

            StringBuilder result = new StringBuilder();
            for (var i = 0; i < 6; i++)
            {
                int charVal = 65 + UnityEngine.Random.Range(0, 24);
                result.Append((char)charVal);
            }
            return result.ToString();
        }
        private void Update()
        {
            if(Input.GetKeyUp(_ExitKey))
            {
                Debug.Log("Exit application.");
                Application.Quit();
            }
        }

    }
}
