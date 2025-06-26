#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class CreateRegionResult
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public IMResult Result { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string RegionGuid { get; set; }// 分区Guid

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public int RegionId { get; set; }// 分区Id
    }

    // 分区消息
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class RegionChatMsg
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string RegionGuid { get; set; }// 所属分区Guid

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string SenderGuid { get; set; }// 发送者Id

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string SenderNickName { get; set; }// 发送者昵称

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string SenderIcon { get; set; }// 发送者头像

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string Msg { get; set; }// 消息内容

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public DateTime Dt { get; set; }// 发送时间

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public GenderType SenderGender { get; set; }

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public int SenderHeadFrameId { get; set; } // 发送者头像框id

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public int SenderTitleId { get; set; }// 发送者称号id

        [ProtoMember(10)]
#if !DEF_CLIENT
        [Id(9)]
#endif
        public int SenderChatBubbleId { get; set; }// 发送者泡泡id
    }

    // 分区信息
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class Region
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string RegionGuid { get; set; }// 分区Guid

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public int RegionId { get; set; }// 分区Id

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string RegionName { get; set; }// 分区名称

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public int PlayerNum { get; set; }// 分区人数

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public DateTime Dt { get; set; }// 分区创建的时间点

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public bool IsActive { get; set; }// 是否激活

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public int Merge2RegionId { get; set; }// 失活后合并入的新分区Id
    }
}