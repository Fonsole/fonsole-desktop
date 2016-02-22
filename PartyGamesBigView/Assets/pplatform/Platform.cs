using UnityEngine;
using System.Collections;
using System;
using System.Text;

using System.Collections.Generic;
using PPlatform.Helper;
using UnityEngine.SceneManagement;
using System.Linq;

using Luz.Tools;

namespace PPlatform
{
    /// <summary>
    /// Main class giving access to everything shared between multiple games + to the network.
    /// 
    /// 
    /// </summary>
    public class Platform : UnitySingleton<Platform>
    {
        public static readonly int VIEW_USER_ID = 0;
        public static readonly int HOST_CONTROLLER_USER_ID = 1;
        public static readonly int MAX_CONTROLLERS = 9;
        private int mNextUserId = HOST_CONTROLLER_USER_ID; //start with the host controller user id


        public KeyCode _ExitKey = KeyCode.Escape;

        private ANetgroup mNetgroup = null;
        private string mActiveName = "gamelist";
        /// <summary>
        /// Key is the user id
        /// </summary>
        private Dictionary<int, Controller> mController = new Dictionary<int, Controller>();
        private Dictionary<int, int> mConnectionIdToUserId = new Dictionary<int, int>();

        private string mGameCode;

        public string GameCode
        {
            get { return mGameCode; }
        }

        public Dictionary<int, Controller> Controllers
        {
            get { return mController; }
        }

        public IEnumerable<Controller> ActiveControllers
        {
            get
            {
                return Platform.Instance.Controllers.Values.Where((x) => { return x.IsAvailable; });
            }
        }
        private bool mIsConnected = false;
        public bool IsConnected
        {
            get
            {
                return mIsConnected;
            }
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

        string TAG_SERVER_FULL = "PLATFORM_SERVER_FULL";
        string TAG_NAME_IN_USE = "PLATFORM_NAME_IN_USE";


        private int mOwnConnectionId = -1;
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

            DebugConsole.ActivateConsole();
        }

        private void OnGUI()
        {
            DebugConsole.DrawConsole();
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
                //remove controller
                DeactivateController(userId);
            }

        }

        private void ActivateController(int connectionId, int userId, string name)
        {
            if(mController.ContainsKey(userId))
            {
                mController[userId].ConnectionId = connectionId;
                Debug.Log("User reconnected " + mController[userId]);
                mConnectionIdToUserId[connectionId] = userId;
            }
            else
            {
                Controller c = new Controller(connectionId, userId, name);
                Debug.Log("Added new " + c);
                mController.Add(userId, c);
                mConnectionIdToUserId[connectionId] = userId;
            }
        }
        public void DeactivateController(int userId)
        {
            Debug.Log("Deactivate " + mController[userId]);
            int connectionId = mController[userId].ConnectionId;
            mController[userId].ConnectionId = -1;
            mConnectionIdToUserId.Remove(connectionId);
        }
        public int ConnectionIdToUserId(int connectionId)
        {
            if (connectionId == mOwnConnectionId)
                return VIEW_USER_ID;

            if (connectionId == -1 || mConnectionIdToUserId.ContainsKey(connectionId) == false)
                return -1;
            return mConnectionIdToUserId[connectionId];
        }

        public int UserIdToConnectionId(int userId)
        {
            if (userId == VIEW_USER_ID)
                return mOwnConnectionId;

            if (userId == -1 || mController.ContainsKey(userId) == false)
                return -1;
            return mController[userId].ConnectionId;
        }
        private void OnNetgroupMessageInternal(ANetgroup.SignalingMessageType type, int conId, string content)
        {
            //Debug.Log("Message type: " + type + " con id: " + conId + " content: " + content);
            if (type == ANetgroup.SignalingMessageType.Connected)
            {
                mOwnConnectionId = conId;
                mGameCode = content;
                mIsConnected = true;
            }
            else if (type == ANetgroup.SignalingMessageType.Closed)
            {
                mIsConnected = false;
            }
            else if (type == ANetgroup.SignalingMessageType.UserJoined)
            {
                //send out the view discovery message to the new user so the controller knows how to contact the view

                //user isn't registered as controller yet -> user intenal send
                SendInternal(TAG_VIEW_DISCOVERY, "", conId);
            }
            else if (type == ANetgroup.SignalingMessageType.UserLeft)
            {
                int userId = ConnectionIdToUserId(conId);

                //only forward if the connection had a user id. if not the user wasn't known in the first place
                //and probably failed to register due to too many players, wrong version, ....
                if(userId != -1)
                    HandlePlatformMessage(TAG_CONTROLLER_LEFT, null, userId);
            }
            else if (type == ANetgroup.SignalingMessageType.UserMessage)
            {
                PlatformMessage pm = JsonWrapper.FromJson<PlatformMessage>(content);

                //special platform messages which are handled before controllers are fully registerd
                if (pm.tag == TAG_CONTROLLER_REGISTER)
                {

                    //server full?
                    if (ActiveControllers.Count() >= MAX_CONTROLLERS)
                    {
                        //user isn't registered as controller yet -> user intenal send
                        SendInternal(TAG_SERVER_FULL, "", conId);
                    }
                    else
                    {
                        //parse the message
                        ControllerRegisterMessage msg = JsonWrapper.FromJson<ControllerRegisterMessage>(pm.content);


                        //name is use by active controller? 
                        if (mController.Values.Where((x) => { return x.IsAvailable && x.Name == msg.name; }).Any())
                        {
                            //someone is online with the same name (an active user!) -> can't login

                            SendInternal(TAG_NAME_IN_USE, "", conId);
                        }
                        else
                        {
                            //all fine -> allow connection and send out discovery message so everyone else knows
                            //a new controller is available
                            //send out discovery broadcast (which will be received locally too and handled below)

                            int userId = -1;
                            foreach (var v in mController)
                            {
                                if (v.Value.Name == msg.name)
                                {
                                    //same name, same user
                                    userId = v.Value.UserId;
                                }
                            }
                            if (userId == -1)
                            {
                                //couldn't find the user based on his name -> give new id
                                userId = mNextUserId;
                                mNextUserId++;
                            }

                            if (string.IsNullOrEmpty(msg.name))
                                msg.name = "player " + userId;

                            Debug.Log("ControllerRegisterMessage received");
                            ControllerDiscoveryMessage discoveryMsg = new ControllerDiscoveryMessage();
                            discoveryMsg.connectionId = conId;
                            discoveryMsg.userId = userId;
                            discoveryMsg.name = msg.name;

                            //send a broadcast that a new user was registered telling the user id
                            SendInternal(TAG_CONTROLLER_DISCOVERY, JsonWrapper.ToJson(discoveryMsg));


                            //DO NOT PASS THIS MESSAGE TO HandlePlatformMessage. this message is internal only
                            //wait until controller discovery is received.
                        }
                    }
                }
                else if (pm.tag == TAG_CONTROLLER_DISCOVERY)
                {
                    //add controller to the list
                    ControllerDiscoveryMessage discoveryMsg = JsonWrapper.FromJson<ControllerDiscoveryMessage>(pm.content);

                    ActivateController(discoveryMsg.connectionId, discoveryMsg.userId, discoveryMsg.name);
                    Send(TAG_ENTER_GAME, mActiveName, discoveryMsg.userId);


                    //controller is known now -> handle the messages
                    HandlePlatformMessage(pm.tag, pm.content, discoveryMsg.userId);

                }
                else
                {
                    int userId = ConnectionIdToUserId(conId);

                    if (userId != -1)
                    {
                        HandlePlatformMessage(pm.tag, pm.content, userId);
                    }
                    else
                    {
                        Debug.LogWarning("Received a message from an unregistered connection " + conId + " content: " + content);
                    }
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
            if (lToUserId != -1 && connectionId == -1)
            {
                Debug.Log("Can't send message to the given user. User isn't connected.");
            }
            else
            {
                SendInternal(JsonWrapper.ToJson(pm), connectionId);
            }
        }

        /// <summary>
        /// Sends a message using the connection id. This is used to send messages
        /// before a user gets registered as controller and thus won't have a user id yet.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="content"></param>
        /// <param name="lConnectionId"></param>
        private void SendInternal(string tag, string content, int lConnectionId = -1)
        {
            PlatformMessage pm = new PlatformMessage();
            pm.tag = tag;
            pm.content = content;

            SendInternal(JsonWrapper.ToJson(pm), lConnectionId);
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
