#if !DEF_CLIENT
using Orleans;
#endif
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    [Component("IMPlayer")]
    public partial interface IComponentStateIMPlayer : IComponentState
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

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "GenderType.Unknow")]
        GenderType Gender { get; set; }// 性别

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        string Icon { get; set; }// 头像

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
        string NickName { get; set; }// 昵称

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
        long Gold { get; set; }// 金币

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "3000")]
        long Diamond { get; set; }// 钻石

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "2333")]
        int Exp { get; set; }// 经验

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "32")]
        int Lv { get; set; }// 等级

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

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
        string InviteId { get; set; }// 邀请Id

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "DateTime.MinValue")]
        DateTime LastLoginDt { get; set; }// 最后登录时间

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "DateTime.MinValue")]
        DateTime JoinDt { get; set; }// 加入时间
    }
}