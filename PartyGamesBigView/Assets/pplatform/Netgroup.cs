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
            mSocket.Emit("user message", m);
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
                Debug.Log("OnDestroy: Disconnecting");
                mSocket.Disconnect();
                mSocket.Close();
                mSocket.Off("connect_timeout");
                mSocket.Off("connect_error");
                mSocket.Off("disconnect");
                mSocket.Off("connect");
                mSocket.Off("room opened");
                mSocket.Off("room joined");
                mSocket.Off("user message");
                mSocket.Off("user joined");
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
            mSocket.On("connect_timeout", OnClose);
            mSocket.On("connect_error", OnClose);
            mSocket.On("disconnect", OnClose);


            //Socket io for unity doesn't seem to implement this the same way as the web version :/
            mSocket.On("connect", () =>
            {
                Debug.Log("Connected to server! Opening room");
                mSocket.Emit("open room", mRoomName);

            });

            //custom messages
            mSocket.On("room opened", (ev) =>
            {
                Debug.Log("Room opened " + ev);
                AddEvent(SignalingMessageType.Connected, ev);
            });


            mSocket.On("room joined", (ev) =>
            {
                Debug.Log("Room joined " + ev);
                AddEvent(SignalingMessageType.Connected, ev);
            });

            mSocket.On("user message", (ev) =>
            {
                AddEvent(SignalingMessageType.UserMessage, ev);
            });
            mSocket.On("user joined", (ev) =>
            {
                Debug.Log(ev);
                AddEvent(SignalingMessageType.UserJoined, ev);
            });
            mSocket.On("user left", (ev) =>
            {
                AddEvent(SignalingMessageType.UserLeft, ev);
            });
        }
    }

}
