#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    public enum MailStateType
    {
        UnRead = 0,
        Readed = 1,
        Recived = 2
    }

    // 邮件附件
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class MailAttachment
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public int ItemId { get; set; }// 道具类型Id

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string ItemObjId { get; set; }// 道具实例Id，可为空，领取附件时需要实例化该道具

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public int ItemCount { get; set; }// 道具数量

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public int ItemFromType { get; set; }// 道具获取来源
    }

    // 邮件
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class Mail
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string MailGuid { get; set; }// 邮件Guid

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string SenderMail { get; set; }// 发送者邮箱地址

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
        public string Title { get; set; }// 邮件标题

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string Msg { get; set; }// 邮件内容

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public DateTime Dt { get; set; }

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public List<MailAttachment> Attachments { get; set; }// 附件        

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public int State { get; set; }// 0未读  1已读  2已领取
    }

    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class ChannelMail
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string PlayerGuid;

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public List<MailAttachment> AttachmentList { get; set; }
    }

    // 系统邮件
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class SystemMail
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string MailGuid { get; set; }// 邮件Guid

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public IMTargetType TargetType { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public List<string> RegionIdList { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public List<string> PlayerIdList { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public DateTime Dt { get; set; }

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public DateTime ExpireDt { get; set; }

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public Mail Mail { get; set; }

#if !DEF_CLIENT
        public void From(DataSystemMail from)
        {
            MailGuid = from._id;
            TargetType = from.TargetType;
            RegionIdList = from.RegionIdList;
            PlayerIdList = from.PlayerIdList;
            Dt = from.Dt;
            ExpireDt = from.ExpireDt;
            Mail = from.Mail;
        }
#endif
    }
}