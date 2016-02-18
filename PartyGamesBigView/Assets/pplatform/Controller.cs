using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPlatform
{
    public class Controller
    {
        private int mUserId;
        public int UserId
        {
            get { return mUserId; }
        }

        private int mConnectionId = -1;
        public int ConnectionId
        {
            get
            {
                return mConnectionId;
            }
            set
            {
                mConnectionId = value;
            }
        }

        public bool IsAvailable
        {
            get { return mConnectionId != -1; }
        }

        private string mName;
        public string Name
        {
            get { return mName; }
        }

        public Controller(int connectionId, int userId, string name)
        {
            mConnectionId = connectionId;
            mUserId = userId;
            mName = name;
        }


        public override string ToString()
        {
            return "Controller[id:" + mUserId + " name:" + mName + "]";
        }


    }
}
