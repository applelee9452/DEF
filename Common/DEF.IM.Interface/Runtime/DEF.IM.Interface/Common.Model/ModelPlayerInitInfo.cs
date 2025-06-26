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
    public class PlayerInitInfo
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
        public ulong UId { get; set; }// UId

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string RegionGuid { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public int RegionId { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public DateTime RegionDt { get; set; }
    }
}