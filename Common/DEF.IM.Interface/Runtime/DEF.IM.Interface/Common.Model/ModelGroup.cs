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
    public partial class CreateGroupResult
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
        public string GroupGuid { get; set; }// 群组Guid

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string GroupName { get; set; }// 群组名
    }

    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class GroupMember
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string PlayerGuid { get; set; }// 玩家Guid

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string NickName { get; set; }// 玩家昵称

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Icon { get; set; }// 玩家头像
    }

    // 群组消息
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class GroupChatMsg
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string GroupGuid { get; set; }// 所属群组Id

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
        public int SenderHeadFrameId { get; set; }

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

        [ProtoMember(11)]
#if !DEF_CLIENT
        [Id(10)]
#endif
        public int FamilyCareer { get; set; }// 发送者家族职位
    }
}