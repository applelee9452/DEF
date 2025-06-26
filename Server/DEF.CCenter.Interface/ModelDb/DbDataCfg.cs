using ProtoBuf;
using System.Collections.Generic;

namespace DEF.CCenter;

[ProtoContract]
[Serializable]
[GenerateSerializer]
public class DataCfg
{
    [ProtoMember(1)]
    [Id(0)]
    public string _id { get; set; }// 与DataNameSpace _id相同

    [ProtoMember(2)]
    [Id(1)]
    public Dictionary<string, string> MapCfg { get; set; }
}