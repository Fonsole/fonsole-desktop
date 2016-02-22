using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using PPlatform.Helper;

namespace PPlatform
{
    public abstract class ANetgroup : MonoBehaviour, PPlatform.INetgroup
    {
        public enum SignalingMessageType
        {
            Invalid = 0,
            Connected = 1,
            Closed = 2,
            UserMessage = 3,
            UserJoined = 4,
            UserLeft = 5,
            OpenRoom = 6,
            JoinRoom = 7
        };


        //private struct RoomInfoMessage
        //{
        //    public string name;
        //    public int id;
        //}
        //private struct UserMessage
        //{
        //    public int id;
        //    public string content;
        //}
        //private struct MsgEventData
        //{
        //    private SignalingMessageType mType;
        //    public SignalingMessageType Type
        //    {
        //        get { return mType; }
        //    }

        //    private object mContent;
        //    public object Content
        //    {
        //        get { return mContent; }
        //    }


        //    public MsgEventData(SignalingMessageType lType, object lContent)
        //    {
        //        mType = lType;
        //        mContent = lContent;
        //    }
        //}


        public enum ConnectionState
        {
            NotConnected,
            Connecting,
            Connected
        }

        public static readonly string MESSAGE_NAME = "SMSG";

        public struct SMessage
        {
            public SignalingMessageType type;
            public string content;
            public int id;

            public SMessage(SignalingMessageType t, string s, int i)
            {
                this.type = t;
                this.content = s;
                this.id = i;
            }
        }


        private Action<SignalingMessageType, int, string> mEventHandler = null;
        private Queue<SMessage> mEventQueue = new Queue<SMessage>();
        private ConnectionState mConnectionState = ConnectionState.NotConnected;

        private int mOwnId = -1;
        private bool mRoomOwner = false;

        public bool RoomOwner
        {
            get { return mRoomOwner; }
        }
        private string mRoomName = null;

        public string RoomName
        {
            get { return mRoomName; }
        }


        // Update is called once per frame
        private void Update()
        {
            HandleEvents();

        }
        public void SendMessageTo(string message, int userid)
        {
            SMessage msg = new SMessage();
            msg.type = SignalingMessageType.UserMessage;
            msg.content = message;
            msg.id = userid;
            //Debug.Log("Snd " + msg);
            SendMessageViaSocketIo(msg);
        }

        private void HandleEvents()
        {
            lock (mEventQueue)
            {
                while (mEventQueue.Count > 0)
                {
                    SMessage data = mEventQueue.Dequeue();
                    HandleEvent(data);
                }
            }

        }

        private void HandleEvent(SMessage message)
        {

            if (message.type == SignalingMessageType.Connected)
            {
                mConnectionState = ConnectionState.Connected;
                mOwnId = message.id;

            }
            else if (message.type == SignalingMessageType.Closed)
            {
                mConnectionState = ConnectionState.NotConnected;
            }
                
            DeliverEvent(message.type, message.id, message.content);

            
        }

        private void DeliverEvent(SignalingMessageType type, int relatedConId, string content)
        {
            if (mEventHandler != null)
            {
                mEventHandler(type, relatedConId, content);
            }
        }
        protected void AddEvent(SMessage msg)
        {
            lock (mEventQueue)
                mEventQueue.Enqueue(msg);
        }
        private void OnDestroy()
        {
            Cleanup();
        }


        protected abstract void InitSocket();
        protected abstract void SendMessageViaSocketIo(SMessage message);
        protected abstract void Cleanup();

        //private void Cleanup()
        //{

        //    if (mSocket != null)
        //    {
        //        Debug.Log("OnDestroy: unregister all handlers");
        //        //if the connectiong is buggy e.g. it is missing incomming events then disconnect often causes unity to stall completly
        //        //the library seem to block this call until the server replied which never happens
        //        //the server registered the disconnect already while this keeps blocking forever
        //        //mSocket.Disconnect();
        //        //mSocket.Close();


        //        //close causes the same bug. just unregister handlers for now

        //        mSocket.Off(Socket.EVENT_CONNECT);
        //        mSocket.Off(Socket.EVENT_CONNECT_TIMEOUT);
        //        mSocket.Off(Socket.EVENT_CONNECT_ERROR);
        //        mSocket.Off(Socket.EVENT_DISCONNECT);
        //        mSocket.Off(Socket.EVENT_ERROR);
        //        mSocket.Off(Socket.EVENT_RECONNECT);
        //        mSocket.Off(Socket.EVENT_RECONNECT_ATTEMPT);
        //        mSocket.Off(Socket.EVENT_RECONNECT_ERROR);
        //        mSocket.Off(Socket.EVENT_RECONNECT_FAILED);
        //        mSocket.Off(Socket.EVENT_RECONNECTING);

        //        mSocket.Off(EVENT_ROOM_OPENED);
        //        mSocket.Off(EVENT_ROOM_JOINED);
        //        mSocket.Off(EVENT_USER_MESSAGE);
        //        mSocket.Off(EVENT_USER_JOINED);
        //        mSocket.Off(EVENT_USER_LEFT);
        //        mSocket = null;
        //        //m.Close();


        //        Debug.Log("Cleanup complete");
        //    }

        //}


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
            }
        }

        public void Close()
        {
            Cleanup();
        }



        //private void OnClose(object ev)
        //{
        //    Debug.Log("Connection closed" + ev);
        //    AddEvent(SignalingMessageType.Closed, ev);
        //}
        //Manager m;
        //private void InitSocket()
        //{

        //    IO.Options opt = new IO.Options();
        //    opt.AutoConnect = false;
        //    opt.Reconnection = false;
        //    opt.ForceNew = true;
        //    opt.Timeout = 10000;


        //    //m = new Manager(new Uri("http://localhost:3001"), opt);
        //    mSocket = IO.Socket("http://localhost:3001", opt);
        //    //mSocket = m.Socket("/");
        //    mSocket.On(Socket.EVENT_CONNECT_TIMEOUT, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_CONNECT_TIMEOUT); OnClose(o); });
        //    mSocket.On(Socket.EVENT_CONNECT_ERROR, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_CONNECT_ERROR); OnClose(o); });
        //    mSocket.On(Socket.EVENT_DISCONNECT, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_DISCONNECT); OnClose(o); });
        //    mSocket.On(Socket.EVENT_ERROR, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_ERROR); OnClose(o); });
        //    mSocket.On(Socket.EVENT_RECONNECT, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_RECONNECT); });
        //    mSocket.On(Socket.EVENT_RECONNECT_ATTEMPT, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_RECONNECT_ATTEMPT); });
        //    mSocket.On(Socket.EVENT_RECONNECT_ERROR, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_RECONNECT_ERROR); });
        //    mSocket.On(Socket.EVENT_RECONNECT_FAILED, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_RECONNECT_FAILED); });
        //    mSocket.On(Socket.EVENT_RECONNECTING, (o) => { Debug.Log("Socket.io event:" + Socket.EVENT_RECONNECTING); });
        //    //Socket io for unity doesn't seem to implement this the same way as the web version :/
        //    mSocket.On(Socket.EVENT_CONNECT, () =>
        //    {
        //        Debug.Log("Connected to server! Opening room");
        //        mSocket.Emit("open room", mRoomName);

        //    });


        //    //custom messages
        //    mSocket.On(EVENT_ROOM_OPENED, (ev) =>
        //    {
        //        Debug.Log("Room opened " + ev);
        //        AddEvent(SignalingMessageType.Connected, ev);
        //    });
        //    mSocket.On(EVENT_ROOM_JOINED, (ev) =>
        //    {
        //        Debug.Log("Room joined " + ev);
        //        AddEvent(SignalingMessageType.Connected, ev);
        //    });
        //    mSocket.On(EVENT_USER_JOINED, (ev) =>
        //    {
        //        Debug.Log(ev);
        //        AddEvent(SignalingMessageType.UserJoined, ev);
        //    });
        //    mSocket.On(EVENT_USER_LEFT, (ev) =>
        //    {
        //        AddEvent(SignalingMessageType.UserLeft, ev);
        //    });
        //    mSocket.On(EVENT_USER_MESSAGE, (ev) =>
        //    {
        //        AddEvent(SignalingMessageType.UserMessage, ev);
        //    });
        //}
    }

}
