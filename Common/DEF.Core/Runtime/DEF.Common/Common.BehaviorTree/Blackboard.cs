

using System.Collections.Generic;

namespace DEF
{
    public class Blackboard
    {

        Dictionary<string, object> MapData = new();


        public void setData(string key, object data)
        {
            MapData[key] = data;
        }


        public object getData(string key)
        {
            MapData.TryGetValue(key, out var data);
            return data;
        }


        public bool hasData(string key)
        {
            return MapData.ContainsKey(key);
        }
    }
}