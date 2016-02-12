using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPlatform
{
    public class Controller
    {
        private int mId;
        public int Id
        {
            get { return mId; }
        }

        private string mName;
        public string Name
        {
            get { return mName; }
        }

        public Controller(int id, string name)
        {
            mId = id;
            mName = name;
        }


        public override string ToString()
        {
            return "Controller[id:" + mId + " name:" + mName + "]";
        }
    }
}
