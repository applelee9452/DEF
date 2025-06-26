#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DEF.IM;

public class DataMailAttachment
{
    public long Gold { get; set; }
    public long Diamond { get; set; }
}

public class DataMail
{
    public string _id { get; set; }
    public string SenderGuid { get; set; }
    public string RecverGuid { get; set; }
    public DateTime CreateTime { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DataMailAttachment Attachment { get; set; }// 邮件附件
    public List<string> TitleParamsList { get; set; }// title的format用到的参数列表
    public List<string> ContentParamsList { get; set; }// content的format用到的参数列表
}


public partial class DataSystemMail
{
    public string _id { get; set; }
    public IMTargetType TargetType { get; set; }
    public List<string> RegionIdList { get; set; }
    public List<string> PlayerIdList { get; set; }
    public DateTime Dt { get; set; }
    public DateTime ExpireDt { get; set; }
    public Mail Mail { get; set; }
    public void From(SystemMail from)
    {
        _id = from.MailGuid;
        TargetType = from.TargetType;
        RegionIdList = from.RegionIdList;
        PlayerIdList = from.PlayerIdList;
        Dt = from.Dt;
        ExpireDt = from.ExpireDt;
        Mail = from.Mail;
    }
}

#endif