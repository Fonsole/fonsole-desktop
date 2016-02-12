using Newtonsoft.Json;
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
            return JsonConvert.SerializeObject(msg);
        }


        public static T FromJson<T>(string msg)
        {
            return JsonConvert.DeserializeObject<T>(msg);
        }
    }
}
