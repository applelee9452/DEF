using ProtoBuf;
using System.Collections.Generic;

namespace DEF.CCenter;

[ProtoContract]
[Serializable]
[GenerateSerializer]
public class DataNameSpace
{
    [ProtoMember(1)]
    [Id(0)]
    public string _id { get; set; }

    [ProtoMember(2)]
    [Id(1)]
    public string NameSpace { get; set; }

    [ProtoMember(3)]
    [Id(2)]
    public string Desc { get; set; }
}