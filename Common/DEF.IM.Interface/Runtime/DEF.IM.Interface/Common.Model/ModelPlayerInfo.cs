#if !DEF_CLIENT
using Orleans;
#endif
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    // 玩家信息
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public class PlayerInfo
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string PlayerGuid { get; set; }// Id

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string NickName { get; set; }// 昵称

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Icon { get; set; }// 头像

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string IpAddress { get; set; }// IP所在地

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public ulong UId { get; set; }// IP所在地
    }
}