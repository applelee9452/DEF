#if DEF_CLIENT

using ProtoBuf;
using System;
using System.IO;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ProtobufBaseTypeRegisterAttribute : Attribute
{
}

public static class ProtobufHelper
{
    public static T Merge1<T>(byte[] bytes, T instance)
    {
        using (MemoryStream stream = new(bytes))
        {
            return Serializer.Merge<T>(stream, instance);
        }
    }

    public static object FromBytes(Type type, byte[] bytes, int index, int count)
    {
        using (MemoryStream stream = new(bytes, index, count))
        {
            return Serializer.Deserialize(type, stream);
        }
    }

    public static T FromBytes<T>(byte[] bytes, int index, int count)
    {
        using (MemoryStream stream = new(bytes, index, count))
        {
            return Serializer.Deserialize<T>(stream);
        }
    }

    public static object FromBytes(Type type, byte[] bytes)
    {
        using (MemoryStream stream = new(bytes))
        {
            return Serializer.Deserialize(type, stream);
        }
    }

    public static T FromBytes<T>(byte[] bytes)
    {
        using (MemoryStream stream = new(bytes))
        {
            return Serializer.Deserialize<T>(stream);
        }
    }

    public static byte[] ToBytes(object message)
    {
        using (MemoryStream stream = new())
        {
            Serializer.Serialize(stream, message);
            return stream.ToArray();
        }
    }

    public static void ToStream(object message, MemoryStream stream)
    {
        Serializer.Serialize(stream, message);
    }

    public static object FromStream(Type type, MemoryStream stream)
    {
        return Serializer.Deserialize(type, stream);
    }
}

#endif