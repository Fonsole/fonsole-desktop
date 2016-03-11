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

        public string _Url = "ws://localhost:3001/socket.io/?EIO=4&transport=websocket";
        

        private void Awake()
        {
        }

        protected override void InitSocket()
        {
            mSocket = gameObject.AddComponent<SocketIOComponent>();
            mSocket.url = _Url;
            Debug.Log("Trying to connect to server " + _Url);
            mSocket.On("open", OnOpen);
            mSocket.On("connect", OnConnected);
            mSocket.On(MESSAGE_NAME, OnMessage);

            mSocket.Connect();
        }

        private void OnMessage(SocketIOEvent e)
        {
            string json = e.data.ToString();
            Debug.Log("REC: " + json);
            SMessage msg = JsonWrapper.FromJson<SMessage>(json);
            AddEvent(msg);
        }


        private void OnOpen(SocketIOEvent e)
        {
            Debug.Log("websocket opened ");
        }
        private void OnConnected(SocketIOEvent e)
        {
            Debug.Log("connected " + e);

            if(RoomOwner)
            {
                Send(new SMessage(SignalingMessageType.OpenRoom, RoomName, -1));
                Debug.Log("Opening room");
            }
            else
            {
                Debug.LogError("Join rooms not yet implemented.");
            }
        }

        private void Send(SMessage msg)
        {
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



        private string Escape(string s)
        {

            s = s.Replace("\\\"", "\"");
            return s;
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
