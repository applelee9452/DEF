using ProtoBuf;

namespace DEF.UCenter;

[ProtoContract]
public class AppAccountDataInfo
{
    [ProtoMember(0)]
    public string AccountId { get; set; }

    [ProtoMember(1)]
    public string AppId { get; set; }

    [ProtoMember(2)]
    public string AppSecret { get; set; }

    [ProtoMember(3)]
    public string Data { get; set; }
}

[ProtoContract]
#if !DEF_CLIENT
[GenerateSerializer]
#endif
public class AppAccountDataResponse
{
    [ProtoMember(1)]
#if !DEF_CLIENT
    [Id(0)]
#endif
    public string AccountId { get; set; }

    [ProtoMember(2)]
#if !DEF_CLIENT
    [Id(1)]
#endif
    public string AppId { get; set; }

    [ProtoMember(3)]
#if !DEF_CLIENT
    [Id(2)]
#endif
    public string Data { get; set; }

    [ProtoMember(4)]
#if !DEF_CLIENT
    [Id(3)]
#endif
    public Dictionary<string, string> DataDictionary { get; set; }
}