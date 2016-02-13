using UnityEngine;
using System.Collections;

using System;
using WebSocket4Net;
using Quobject.SocketIoClientDotNet.Client;
using System.Collections.Generic;
using Newtonsoft.Json;
using PPlatform.Helper;

namespace PPlatform
{
    public class Netgroup : MonoBehaviour
    {
        enum ConnectionState
        {
            NotConnected,
            Connecting,
            Connected
        };


        public enum SignalingMessageType
        {
            Invalid = 0,
            Connected = 1,
            Closed = 2,
            UserMessage = 3,
            UserJoined = 4,
            UserLeft = 5
        };


        private struct RoomInfoMessage
        {
            public string name;
            public int id;
        }


        private struct UserMessage
        {
            public int id;
            public string content;
        }
        private struct MsgEventData
        {
            private SignalingMessageType mType;
            public SignalingMessageType Type
            {
                get { return mType; }
            }

            private object mContent;
            public object Content
            {
                get { return mContent; }
            }


            public MsgEventData(SignalingMessageType lType, object lContent)
            {
                mType = lType;
                mContent = lContent;
            }
        }



        public static readonly string EVENT_ROOM_OPENED = "room opened";
        public static readonly string EVENT_ROOM_JOINED = "room joined";
        public static readonly string EVENT_USER_JOINED = "user joined";
        public static readonly string EVENT_USER_LEFT = "user left";
        public static readonly string EVENT_USER_MESSAGE = "user message";

        private ConnectionState mConnectionState = ConnectionState.NotConnected;
        private Action<SignalingMessageType, int, string> mEventHandler = null;
        private Queue<MsgEventData> mEventQueue = new Queue<MsgEventData>();
        private Socket mSocket = null;

        private int mOwnId = -1;
        private bool mRoomOwner = false;
        private string mRoomName = null;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            HandleEvents();

        }
        public void SendMessageTo(string message, int userid)
        {
            UserMessage msg = new UserMessage();
            msg.content = message;
            msg.id = userid;

            string m = JsonConvert.SerializeObject(msg);
            mSocket.Emit(EVENT_USER_MESSAGE, m);
            Debug.Log("Send " + m);
        }
        private void HandleEvents()
        {


            lock (mEventQueue)
            {
                while (mEventQueue.Count > 0)
                {
                    MsgEventData data = mEventQueue.Dequeue();
                    HandleEvent(data.Type, data.Content);
                }
            }

        }

        private void HandleEvent(SignalingMessageType lType, object message)
        {

            if (lType == SignalingMessageType.Connected)
            {
                mConnectionState = ConnectionState.Connected;
                string lContent = message as string;
                RoomInfoMessage msg = JsonConvert.DeserializeObject<RoomInfoMessage>(lContent as string);

                //mRoomName = msg.name;
                int lConnectionId = -1;
                lConnectionId = msg.id;
                mOwnId = msg.id;
                lContent = msg.name;
                DeliverEvent(lType, lConnectionId, mRoomName);

            }
            else if (lType == SignalingMessageType.UserJoined || lType == SignalingMessageType.UserLeft)
            {
                int lConnectionId = (int)((long)message);
                DeliverEvent(lType, lConnectionId, null);
            }
            else if (lType == SignalingMessageType.UserMessage)
            {
                UserMessage um = JsonWrapper.FromJson<UserMessage>(message as string);
                DeliverEvent(lType, um.id, um.content); //TODO: fix this
            }
            else if (lType == SignalingMessageType.Closed)
            {
                mConnectionState = ConnectionState.NotConnected;
                DeliverEvent(lType, -1, message as string);
            }
        }

        private void DeliverEvent(SignalingMessageType type, int relatedConId, string content)
        {

            if (mEventHandler != null)
            {
                mEventHandler(type, relatedConId, content);
            }
        }
        private void AddEvent(SignalingMessageType lType, object lContent)
        {
            lock (mEventQueue)
                mEventQueue.Enqueue(new MsgEventData(lType, lContent));
        }
        private void OnDestroy()
        {
            Cleanup();
        }

        private void Cleanup()
        {

            if (mSocket != null)
            {
                Debug.Log("OnDestroy: unregister all handlers");
                //if the connectiong is buggy e.g. it is missing incomming events then disconnect often causes unity to stall completly
                //the library seem to block this call until the server replied which never happens
                //the server registered the disconnect already while this keeps blocking forever
                //mSocket.Disconnect();
                //mSocket.Close();


                //close causes the same bug. just unregister handlers for now

                mSocket.Off(Socket.EVENT_CONNECT);
                mSocket.Off(Socket.EVENT_CONNECT_TIMEOUT);
                mSocket.Off(Socket.EVENT_CONNECT_ERROR);
                mSocket.Off(Socket.EVENT_DISCONNECT);
                mSocket.Off(Socket.EVENT_ERROR);
                mSocket.Off(Socket.EVENT_RECONNECT);
                mSocket.Off(Socket.EVENT_RECONNECT_ATTEMPT);
                mSocket.Off(Socket.EVENT_RECONNECT_ERROR);
                mSocket.Off(Socket.EVENT_RECONNECT_FAILED);
                mSocket.Off(Socket.EVENT_RECONNECTING);

                mSocket.Off(EVENT_ROOM_OPENED);
                mSocket.Off(EVENT_ROOM_JOINED);
                mSocket.Off(EVENT_USER_MESSAGE);
                mSocket.Off(EVENT_USER_JOINED);
                mSocket.Off(EVENT_USER_LEFT);
                mSocket = null;
                //m.Close();


                Debug.Log("Cleanup complete");
            }

        }


        public void Open(string name, Action<SignalingMessageType, int, string> lEventHandler)
        {
            mEventHandler = lEventHandler;
            mRoomName = name;
            mRoomOwner = true;

            if (mConnectionState == ConnectionState.NotConnected)
            {
                Debug.Log("Trying to connect to server ...");
                mConnectionState = ConnectionState.Connecting;
                InitSocket();
                mSocket.Connect();
            }
        }

        public void Close()
        {
            Cleanup();
        }

        private void OnClose(object ev)
        {
            Debug.Log("Connection closed" + ev);
            AddEvent(SignalingMessageType.Closed, ev);
        }
        Manager m;
        private void InitSocket()
        {

            IO.Options opt = new IO.Options();
            opt.AutoConnect = false;
            opt.Reconnection = false;
            opt.ForceNew = true;
            opt.Timeout = 10000;


            //m = new Manager(new Uri("http://localhost:3001"), opt);
            mSocket = IO.Socket("http://localhost:3001", opt);
            //mSocket = m.Socket("/");
            mSocket.On(Socket.EVENT_CONNECT_TIMEOUT, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_CONNECT_TIMEOUT); OnClose(o); });
            mSocket.On(Socket.EVENT_CONNECT_ERROR, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_CONNECT_ERROR); OnClose(o); });
            mSocket.On(Socket.EVENT_DISCONNECT, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_DISCONNECT); OnClose(o); });
            mSocket.On(Socket.EVENT_ERROR, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_ERROR); OnClose(o); });
            mSocket.On(Socket.EVENT_RECONNECT, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_RECONNECT); });
            mSocket.On(Socket.EVENT_RECONNECT_ATTEMPT, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_RECONNECT_ATTEMPT); });
            mSocket.On(Socket.EVENT_RECONNECT_ERROR, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_RECONNECT_ERROR); });
            mSocket.On(Socket.EVENT_RECONNECT_FAILED, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_RECONNECT_FAILED); });
            mSocket.On(Socket.EVENT_RECONNECTING, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_RECONNECTING); });
            //Socket io for unity doesn't seem to implement this the same way as the web version :/
            mSocket.On(Socket.EVENT_CONNECT, () =>
            {
                Debug.Log("Connected to server! Opening room");
                mSocket.Emit("open room", mRoomName);

            });


            //custom messages
            mSocket.On(EVENT_ROOM_OPENED, (ev) =>
            {
                Debug.Log("Room opened " + ev);
                AddEvent(SignalingMessageType.Connected, ev);
            });
            mSocket.On(EVENT_ROOM_JOINED, (ev) =>
            {
                Debug.Log("Room joined " + ev);
                AddEvent(SignalingMessageType.Connected, ev);
            });
            mSocket.On(EVENT_USER_JOINED, (ev) =>
            {
                Debug.Log(ev);
                AddEvent(SignalingMessageType.UserJoined, ev);
            });
            mSocket.On(EVENT_USER_LEFT, (ev) =>
            {
                AddEvent(SignalingMessageType.UserLeft, ev);
            });
            mSocket.On(EVENT_USER_MESSAGE, (ev) =>
            {
                AddEvent(SignalingMessageType.UserMessage, ev);
            });
        }
    }

}
