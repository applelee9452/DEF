using ProtoBuf;

namespace DEF.UCenter;

[ProtoContract]
public class AppConfigInfo
{
    [ProtoMember(0)]
    public string AppId { get; set; }

    [ProtoMember(1)]
    public string Config { get; set; }
}

[ProtoContract]
public class AppConfigResponse
{
    [ProtoMember(0)]
    public string AppId { get; set; }

    [ProtoMember(1)]
    public string Configuration { get; set; }
}