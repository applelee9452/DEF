#if !DEF_CLIENT

using MemoryPack;
using ProtoBuf;

namespace DEF.IM;

public partial class DataCDKey
{
    public void From(CDKey cdkey)
    {
        _id = cdkey.CDKeyGuid;
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
}

[ProtoContract]
[MemoryPackable]
#if !DEF_CLIENT
[GenerateSerializer]
#endif
public partial class DataCDKey
{
    [ProtoMember(1)]
#if !DEF_CLIENT
    [Id(0)]
#endif
    public string _id { get; set; }

    [ProtoMember(2)]
#if !DEF_CLIENT
    [Id(1)]
#endif
    public string Key { get; set; }// 输入Key

    [ProtoMember(3)]
#if !DEF_CLIENT
    [Id(2)]
#endif
    public DateTime CreateDt { get; set; } // 创建时间

    [ProtoMember(4)]
#if !DEF_CLIENT
    [Id(3)]
#endif
    public DateTime ExpireDt { get; set; } // 过期时间

    [ProtoMember(5)]
#if !DEF_CLIENT
    [Id(4)]
#endif
    public IMTargetType TargetType { get; set; }// 目标类型

    [ProtoMember(6)]
#if !DEF_CLIENT
    [Id(5)]
#endif
    public List<string> RegionIdList { get; set; }// 目标 分区 Id List

    [ProtoMember(7)]
#if !DEF_CLIENT
    [Id(6)]
#endif
    public List<string> PlayerIdList { get; set; }// 目标 PlayerId List

    [ProtoMember(8)]
#if !DEF_CLIENT
    [Id(7)]
#endif
    public string MailTitle { get; set; } // 邮件标题

    [ProtoMember(9)]
#if !DEF_CLIENT
    [Id(8)]
#endif
    public string MailDesc { get; set; } // 邮件描述

    [ProtoMember(10)]
#if !DEF_CLIENT
    [Id(9)]
#endif
    public List<CDKeyAttachment> CDKeyAttachmentList { get; set; } // 邮件物品列表

    [ProtoMember(11)]
#if !DEF_CLIENT
    [Id(10)]
#endif
    public bool IsDelete { get; set; } // 是否已删除
}

#endif