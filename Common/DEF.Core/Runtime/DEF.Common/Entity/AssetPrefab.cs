using ProtoBuf;
using System.Collections.Generic;

namespace DEF
{
    [ProtoContract]
    public class ComponentPrefab
    {
        [ProtoMember(1)]
        public string ComponentName { get; set; }

        [ProtoMember(2)]
        public string States { get; set; }// Json

        [ProtoMember(3)]
        public KeyValuePair<string, string> CommonVariables { get; set; }// Common变量组

        [ProtoMember(4)]
        public KeyValuePair<string, string> ClientVariables { get; set; }// Client变量组

        [ProtoMember(5)]
        public KeyValuePair<string, string> ServerVariables { get; set; }// Server变量组
    }

    [ProtoContract]
    public class AssetPrefab
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public Dictionary<int, string> Tags { get; set; }

        [ProtoMember(3)]
        public List<ComponentPrefab> Components { get; set; }

        [ProtoMember(4)]
        public List<AssetPrefab> Children { get; set; }

        public static void Save(AssetPrefab asset_prefab, string path)
        {
            // todo，Yaml序列化
        }

        public static AssetPrefab Load(string path)
        {
            AssetPrefab asset_prefab = null;

            // todo，Yaml反序列化

            return asset_prefab;
        }
    }
}