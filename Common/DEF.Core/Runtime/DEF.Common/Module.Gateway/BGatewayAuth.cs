#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System.Collections.Generic;

namespace DEF.Gateway
{
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class ClientAuthRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string acc_id { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string token { get; set; }// UCenter发放的Token，有过期时间

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string session_id { get; set; }// Tcp建立连接后，主动推送过来的SessionId

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string nick_name { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string channel_id { get; set; }// 默认渠道为空字符串

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string platform { get; set; }// Client平台，目前可以取值Android, iOS, Pc

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public string region { get; set; }// Client区号，即国家码

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public string lan { get; set; }// Client语言

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public string invite_id { get; set; }// 邀请玩家PlayerId

        [ProtoMember(10)]
#if !DEF_CLIENT
        [Id(9)]
#endif
        public string device_id { get; set; }

        [ProtoMember(11)]
#if !DEF_CLIENT
        [Id(10)]
#endif
        public Dictionary<string, string> map_extra_data { get; set; }// 额外数据
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class ClientAuthResponse
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public int result { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string PlayerGuid { get; set; }
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class GatewayAuthResponse
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string PlayerGuid { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public int Gender { get; set; }
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class GatewayAuthedInfo
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string AccId { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string GatewayGuid { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string SessionGuid { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string PlayerGuid { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string ClientIp { get; set; }

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string ClientIpAddress { get; set; }

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public string NickName { get; set; }

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public int Gender { get; set; }// 0=未知，1=男，2=女

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public string ChannelId { get; set; }// 默认渠道为空字符串

        [ProtoMember(10)]
#if !DEF_CLIENT
        [Id(9)]
#endif
        public string Platform { get; set; }// Client平台，目前可以取值Android, iOS, Pc

        [ProtoMember(11)]
#if !DEF_CLIENT
        [Id(10)]
#endif
        public string Region { get; set; }// Client区号，即国家码

        [ProtoMember(12)]
#if !DEF_CLIENT
        [Id(11)]
#endif
        public string Lan { get; set; }// Client语言

        [ProtoMember(13)]
#if !DEF_CLIENT
        [Id(12)]
#endif
        public string InviteId { get; set; }// 邀请玩家PlayerId

        [ProtoMember(14)]
#if !DEF_CLIENT
        [Id(13)]
#endif
        public string DeviceId { get; set; }
    }
}