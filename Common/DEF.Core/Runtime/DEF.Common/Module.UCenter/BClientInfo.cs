#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System;

namespace DEF.UCenter
{
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class ClientInfo
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public DateTime LastLoginDateTime { get; set; }// 最新一次登录的时间

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string LastLoginClientIp { get; set; }// 最新一次登录的客户端Ip

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string LastLoginDeviceId { get; set; }// 最新一次登录的设备Id

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string Identity { get; set; }// 身份证
    }
}