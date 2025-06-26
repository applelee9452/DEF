#if !DEF_CLIENT

using System;

namespace DEF.IM;

public abstract class EventBase : DataBase
{
    public string EventType { get; set; }// 事件类型
    public DateTime EventTm { get; set; }// 事件时间
}

public class EvPlayerOnlineCount : EventBase // 玩家在线数量事件
{
    public long OnlinePlayerCount { get; set; } // 在线玩家数量
    public long OnlineBotCount { get; set; }// 在线Bot数量
}

public class EvPlayerCreate : EventBase // 玩家创建事件
{
    public string PlayerGuid { get; set; } // 玩家Guid
    public long InitGoldAcc { get; set; } // 初始金币
    public long InitDiamond { get; set; } // 初始钻石
    public string ChannelId { get; set; }// 渠道Id
}

public class EvPlayerLoginLogout : EventBase // 玩家登陆登出事件
{
    public string PlayerGuid { get; set; } // 玩家Guid
    public string Action { get; set; }// "Login"，"Logout"
}

// 钻石变化事件
public class EvPlayerDiamondChange : EventBase
{
    public string PlayerGuid { get; set; }// 玩家的Guid  
    public long BeforeChangeDiamond { get; set; }// 改变前钻石总数
    public long AfterChangeDiamond { get; set; }// 改变后钻石总数
    public long ChangeDiamond { get; set; }
    public bool IsBot { get; set; }// 是否是机器人
    public string ParamS { get; set; }
    public bool Result { get; set; }
}

// 积分变化事件
public class EvPlayerPointChange : EventBase
{
    public string PlayerGuid { get; set; }// 玩家的Guid  
    public long BeforeChangePoint { get; set; }// 改变前积分总数
    public long AfterChangePoint { get; set; }// 改变后积分总数
    public long ChangePoint { get; set; }
    public bool IsBot { get; set; }// 是否是机器人
    public string ParamS { get; set; }
    public bool Result { get; set; }
}

public class EvPlayerReportOther : EventBase // 玩家举报
{
    public string ReportPlayer { get; set; } // 举报人PlayerGuid
    public string ReportNickName { get; set; }// 举报人昵称
    public string BeingReportedPlayer { get; set; } // 被举报人PlayerGuid
    public string BeingReportedNickName { get; set; } // 被举报人昵称
}

#endif