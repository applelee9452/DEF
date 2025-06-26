using ProtoBuf;

namespace DEF.UCenter;

[ProtoContract]
public class AppAccountLoginInfo
{
    [ProtoMember(0)]
    public string AppId { get; set; }

    [ProtoMember(1)]
    public string AppSecret { get; set; }

    [ProtoMember(2)]
    public string AccountId { get; set; }

    [ProtoMember(3)]
    public string AccountToken { get; set; }
}

[ProtoContract]
public class AppAccountLoginResponse
{
    [ProtoMember(0)]
    public string AccountId { get; set; }

    [ProtoMember(1)]
    public string AccountName { get; set; }

    [ProtoMember(2)]
    public GenderType Gender { get; set; }

    [ProtoMember(3)]
    public string Icon { get; set; }

    [ProtoMember(4)]
    public string AccountToken { get; set; }

    [ProtoMember(5)]
    public string AccountData { get; set; }

    [ProtoMember(6)]
    public ClientInfo ClientInfo { get; set; }
}