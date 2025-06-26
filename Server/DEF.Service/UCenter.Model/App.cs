using ProtoBuf;

namespace DEF.UCenter;

[ProtoContract]
public class AppInfo
{
    [ProtoMember(0)]
    public string AppId { get; set; }

    [ProtoMember(1)]
    public string AppSecret { get; set; }
}

[ProtoContract]
public class AppResponse
{
    [ProtoMember(0)]
    public string AppId { get; set; }

    [ProtoMember(1)]
    public string AppSecret { get; set; }

    [ProtoMember(2)]
    public string WechatAppId { get; set; }

    [ProtoMember(3)]
    public string WechatAppSecret { get; set; }

    [ProtoMember(4)]
    public string Token { get; set; }
}