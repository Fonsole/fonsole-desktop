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

        private string mName;
        public string Name
        {
            get { return mName; }
        }

        public Controller(int id, string name)
        {
            mUserId = id;
            mName = name;
        }


        public override string ToString()
        {
            return "Controller[id:" + mUserId + " name:" + mName + "]";
        }
    }
}
