using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.SyncDb;

[ProtoContract]
public class ServerItem
{
    [ProtoMember(1)]
    public string ServerId { get; set; }//服务器id

    [ProtoMember(2)]
    public int ServerStat { get; set; }//服务器状态，0：普通，1：爆满

    [ProtoMember(3)]
    public int PlayerLv { get; set; }//玩家等级

    [ProtoMember(4)]
    public DateTime LastPlayTime { get; set; }//最后上线游玩时间戳
}

[ProtoContract]
public class ServerList
{
    [ProtoMember(1)]
    public string CurrentServerId { get; set; }//我当前所在的服务器id

    [ProtoMember(2)]
    public List<ServerItem> MineList { get; set; }//我所在的服务器列表

    [ProtoMember(3)]
    public List<ServerItem> AllList { get; set; }//所有的服务器列表
}

[Component("Player")]
public partial interface IComponentStatePlayer : IComponentState
{
    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    string AccountId { get; set; }// 帐号Guid

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    string PlayerGuid { get; set; }// 玩家Guid

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    ulong UId { get; set; }// UId

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "false")]
    bool IsBot { get; set; }// 是否是机器人

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "0")]
    int BotId { get; set; }// 机器人Id

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    string Icon { get; set; }// 头像

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    string IconFream { get; set; }// 头像框

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    string NickName { get; set; }// 昵称

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "GenderType.Unknow")]
    GenderType Gender { get; set; }// 性别

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    string RegionGuid { get; set; }// 当前所属服务器，分区Guid

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    int RegionId { get; set; }// 当前所属服务器，分区Id

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    DateTime RegionDt { get; set; }// 当前所属服务器，分区开服日期

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    long Gold { get; set; }// 金币

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    long StarDiamond { get; set; }// 星钻

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    long Diamond { get; set; }// 钻石  //需要清除测试数据

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    int TicketTimeStep { get; set; }// 加速券

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    int TicketSkill { get; set; }// 技能券

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    int TicketPartner { get; set; }// 同伴券

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    Dictionary<int, int> DaysReward { get; set; }// 本日奖励

    [Prop(PropType.Default, PropSyncFlag.SyncDbOnly, PropSyncMode.Set, PropCallback.No, "")]
    Dictionary<int, int> Back { get; set; }// 背包

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    float GetOnlineRewardCD { get; set; }//获取在线奖励倒计时 秒

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    int SaveOnlineRewardNumber { get; set; }//现在已存储获取在线礼物的次数 分钟

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    int SaveOfflineRewardNumber { get; set; }//现在已存储获取离线礼物的次数 分钟

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    Dictionary<int, int> OnlineRewardBox { get; set; }//在线礼物箱

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
    Dictionary<int, int> OfflineRewardBox { get; set; }//离线线礼物箱

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    long RechargePoint { get; set; }// 充值点数（==VIPPoint，是总数）

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    bool IsFirstRecharge { get; set; }// 是否是首充

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    string IndividualSignature { get; set; }// 个性签名

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    string ClientIp { get; set; }// Ip

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    string IpAddress { get; set; }// Ip所在地

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    string Province { get; set; }// 省份

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    string City { get; set; }// 城市

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "DateTime.MinValue")]
    DateTime LastLoginDt { get; set; }// 最后登录时间

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "DateTime.MinValue")]
    DateTime LastLogoutDt { get; set; }// 最后下线时间

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "DateTime.MinValue")]
    DateTime JoinDt { get; set; }// 加入时间

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
    string BuffInfo { get; set; }// 技能测试信息

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "0")]
    int ChangeNamePay { get; set; }    //修改名字花费

    [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "0")]
    int ChangeGenderPay { get; set; }   //修改性别花费
}