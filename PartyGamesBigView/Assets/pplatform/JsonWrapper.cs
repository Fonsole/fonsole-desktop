
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPlatform.Helper
{
    public class JsonWrapper
    {
        public static string ToJson(object msg)
        {
            //throw new NotImplementedException();
            return Newtonsoft.Json.JsonConvert.SerializeObject(msg);
        }


        public static T FromJson<T>(string msg)
        {
            //throw new NotImplementedException();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(msg);
        }
    }
}
