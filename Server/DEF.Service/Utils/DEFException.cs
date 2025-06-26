using System;
using System.Runtime.Serialization;

namespace DEF;

[Serializable]
public class DEFException : Exception
{
    public DEFException() : base("Unexpected error.") { }

    public DEFException(string message) : base(message) { }

    public DEFException(string message, Exception innerException) : base(message, innerException) { }

    //protected DEFException(SerializationInfo info, StreamingContext context)
    //    : base(info, context)
    //{
    //}
}