using System.IO;

namespace DEF
{
    public static class EntitySerializer
    {
        public static byte[] Serialize<T>(SerializerType serializer_type, T obj)
        {
            byte[] data = null;

            if (obj == null)
            {
                return data;
            }

            using var ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, obj);
            data = ms.ToArray();

            //if (serializer_type == SerializerType.MemoryPack)
            //{
            //    data = MemoryPack.MemoryPackSerializer.Serialize(obj);
            //}
            //else if (serializer_type == SerializerType.Protobuf)
            //{
            //    using var ms = new MemoryStream();
            //    ProtoBuf.Serializer.Serialize(ms, obj);
            //    data = ms.ToArray();
            //}
            //else if (serializer_type == SerializerType.LitJson)
            //{
            //    string s = LitJson.JsonMapper.ToJson(obj);
            //    data = System.Text.Encoding.UTF8.GetBytes(s);
            //}

            return data;
        }

        public static T Deserialize<T>(SerializerType serializer_type, byte[] data)
        {
            T obj = default;
            if (data == null)
            {
                return obj;
            }
            using var ms = new MemoryStream(data);
            obj = ProtoBuf.Serializer.Deserialize<T>(ms);

            //if (serializer_type == SerializerType.MemoryPack)
            //{
            //    obj = MemoryPack.MemoryPackSerializer.Deserialize<T>(data);
            //}
            //else if (serializer_type == SerializerType.Protobuf)
            //{
            //    using var ms = new MemoryStream(data);
            //    obj = ProtoBuf.Serializer.Deserialize<T>(ms);
            //}
            //else if (serializer_type == SerializerType.LitJson)
            //{
            //    var t = typeof(T);
            //    string s = System.Text.Encoding.UTF8.GetString(data);
            //    LitJson.JsonMapper.ToObject(s, t);
            //}

            return obj;
        }

        public static object[] DeserializeObj(SerializerType serializer_type, System.Type t, byte[] data)
        {
            object[] arr_param = null;

            if (data == null)
            {
                return arr_param;
            }

            using var ms = new MemoryStream(data);
            var so = (SerializeObj)ProtoBuf.Serializer.Deserialize(t, ms);
            arr_param = so.ToObjectArray();

            //if (serializer_type == SerializerType.MemoryPack)
            //{
            //    var so = (SerializeObj)MemoryPack.MemoryPackSerializer.Deserialize(t, data);
            //    arr_param = so.ToObjectArray();
            //}
            //else if (serializer_type == SerializerType.Protobuf)
            //{
            //    using var ms = new MemoryStream(data);
            //    var so = (SerializeObj)ProtoBuf.Serializer.Deserialize(t, ms);
            //    arr_param = so.ToObjectArray();
            //}
            //else if (serializer_type == SerializerType.LitJson)
            //{
            //    string s = System.Text.Encoding.UTF8.GetString(data);
            //    var so = (SerializeObj)LitJson.JsonMapper.ToObject(s, t);
            //    arr_param = so.ToObjectArray();
            //}

            return arr_param;
        }
    }
}