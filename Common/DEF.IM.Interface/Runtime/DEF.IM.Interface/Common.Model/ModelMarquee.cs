#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    public enum IMMarqueeSenderType
    {
        System = 0,
        Player
    }

    public enum IMMarqueePriority
    {
        Normal = 0,
        High
    }

    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class BIMMarquee
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public IMMarqueeSenderType SenderType { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string SenderGuid { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string NickName { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public int VIPLevel { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public IMMarqueePriority Priority { get; set; }

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string Msg { get; set; }

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public string FormatKey { get; set; }// 客户端使用到的key

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public List<string> MsgParamsList { get; set; }
    }

    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class IMMarqueeEx
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public BIMMarquee im_marquee { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public float total_tm { get; set; }// 多久后播

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public float elapsed_tm { get; set; }// 已过多久
    }
}