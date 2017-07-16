using DebugTools;
using PPlatform.Helper;
using SocketIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PPlatform
{
    public class UnityNetgroup : ANetgroup
    {
        private SocketIOComponent mSocket;

        public string _Url = "ws://fonsole.us-3.evennode.com/socket.io/?EIO=4&transport=websocket";


        private bool mReadyForOpenRoom = false;

        private void Awake()
        {
        }

        protected override void InitSocket()
        {
            mSocket = gameObject.AddComponent<SocketIOComponent>();
            mSocket.url = _Url;
            TL.L("Trying to connect to server " + _Url);
            mSocket.On("open", OnOpen);
            mSocket.On("connect", OnConnected);
            mSocket.On(MESSAGE_NAME, OnMessage);

            mSocket.Connect();
        }

        private void OnMessage(SocketIOEvent e)
        {
            string json = e.data.ToString();
            TL.L("REC: " + json);
            SMessage msg = JsonWrapper.FromJson<SMessage>(json);
            AddEvent(msg);
        }


        private void OnOpen(SocketIOEvent e)
        {
            //WARNING: SocketIO plugin calls this from a different thread! this isn't suppose to happen might cause bugs
            Debug.Log("websocket opened " + e);
        }
        private void OnConnected(SocketIOEvent e)
        {
            Debug.Log("connected " + e);
            //WARNING: SocketIO plugin calls this from a different thread! this isn't suppose to happen might cause bugs

            //we just set a flag for now to avoid bugs through multi threading
            lock (mEventQueue)
            {
                mReadyForOpenRoom = true;
            }
        }

        protected override void Update()
        {
            //check for the flag to send out the open room command
            lock (mEventQueue)
            {
                if (mReadyForOpenRoom)
                {
                    mReadyForOpenRoom = false;
                    if (RoomOwner)
                    {
                        Send(new SMessage(SignalingMessageType.GameStarted, "", -1));
                    }
                    else
                    {
                        Debug.LogError("Join rooms not yet implemented.");
                    }
                }

            }

            //will handle the events
            base.Update();
        }

        private void Send(SMessage msg)
        {
            TL.L("SND: " + msg);
            mSocket.Emit(MESSAGE_NAME, ToJsonObject(msg));
        }
        private JSONObject ToJsonObject(SMessage msg)
        {
            //JSONObject jo = new JSONObject(JSONObject.Type.OBJECT);
            //jo["type"] = new JSONObject((int)msg.Type);
            //JSONObject str = new JSONObject(JSONObject.Type.STRING);
            //str.str = msg.Content;
            //jo["content"] = str;
            //jo["id"] = new JSONObject(msg.Id);
            //return jo;
            return new JSONObject(JsonWrapper.ToJson(msg));
        }

        protected override void SendMessageViaSocketIo(SMessage message)
        {
            Send(message);
        }

        protected override void Cleanup()
        {
            if (mSocket != null)
                Destroy(mSocket);
            mSocket = null;
        }
    }
}
