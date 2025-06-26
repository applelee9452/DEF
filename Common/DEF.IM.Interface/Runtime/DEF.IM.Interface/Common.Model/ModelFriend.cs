#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    // 单个好友信息
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public class FriendItem
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
        public string NickName { get; set; }// 昵称

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Icon { get; set; }// 头像
    }

    // 单个申请好友信息
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public class AddFriendItem
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
        public string NickName { get; set; }// 昵称

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Icon { get; set; }// 头像
    }

    // 单个黑名单信息
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public class BlackItem
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
        public string NickName { get; set; }// 昵称

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Icon { get; set; }// 头像
    }

    // 单聊消息，陌生人或好友，发送的
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SingleChatMsgSend
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string SenderGuid { get; set; }// 发送者Id

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string RecverGuid { get; set; }// 接收者Id

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Msg { get; set; }// 消息内容

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public DateTime Dt { get; set; }// 发送时间
    }

    // 单聊消息，陌生人或好友，接收的
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SingleChatMsgRecv
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public ulong MsgId { get; set; }// 消息Id

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
        public string RecverGuid { get; set; }// 接收者Id

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public string RecverNickName { get; set; }// 接收者昵称

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public string RecverIcon { get; set; }// 接收者头像
    }
}