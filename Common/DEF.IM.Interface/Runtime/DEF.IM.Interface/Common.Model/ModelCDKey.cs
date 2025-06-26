#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DEF.IM
{
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class CDKeyAttachment
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public int ItemId { get; set; }// Item模板Id

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string ItemObjId { get; set; }// Item实例Id

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public int ItemCount { get; set; }// Item数量

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string Key { get; set; }// 自定义奖励Key

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string Param1 { get; set; }// 自定义奖励参数1

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string Param2 { get; set; }// 自定义奖励参数2

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public string Param3 { get; set; }// 自定义奖励参数3

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public string Param4 { get; set; }// 自定义奖励参数4
    }

    public enum IMTargetType
    {
        [Description("全体玩家")]
        All = 0,
        [Description("分区玩家")]
        Region = 1,
        [Description("个别玩家")]
        Player = 2
    }

    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class CDKey
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string CDKeyGuid { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Key { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public DateTime CreateDt { get; set; }// 生成时间点

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public DateTime ExpireDt { get; set; }// 过期时间点

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public IMTargetType TargetType { get; set; }// 目标类型

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public List<string> RegionIdList { get; set; }// 目标分区列表

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public List<string> PlayerIdList { get; set; }// 目标玩家列表

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public string MailTitle { get; set; }// 邮件标题

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public string MailDesc { get; set; }// 邮件描述

        [ProtoMember(10)]
#if !DEF_CLIENT
        [Id(9)]
#endif
        public List<CDKeyAttachment> CDKeyAttachmentList { get; set; }// 邮件物品列表

        [ProtoMember(11)]
#if !DEF_CLIENT
        [Id(10)]
#endif
        public bool IsDelete { get; set; }// 是否已删除

#if !DEF_CLIENT
        public void From(DataCDKey cdkey)
        {
            CDKeyGuid = cdkey._id;
            Key = cdkey.Key;
            CreateDt = cdkey.CreateDt;
            ExpireDt = cdkey.ExpireDt;
            TargetType = cdkey.TargetType;
            RegionIdList = cdkey.RegionIdList;
            PlayerIdList = cdkey.PlayerIdList;
            MailTitle = cdkey.MailTitle;
            MailDesc = cdkey.MailDesc;
            CDKeyAttachmentList = cdkey.CDKeyAttachmentList;
            IsDelete = cdkey.IsDelete;
        }
#endif
    }
}