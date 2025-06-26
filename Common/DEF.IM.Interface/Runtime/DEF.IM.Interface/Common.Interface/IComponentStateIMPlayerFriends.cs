#if !DEF_CLIENT
using Orleans;
#endif
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    [Component("IMPlayerFriends")]
    public partial interface IComponentStateIMPlayerFriends : IComponentState
    {
        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
        List<FriendItem> ListFriend { get; set; }// 好友列表，每个好友有一组信息

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
        List<AddFriendItem> ListAddFriend { get; set; }// 申请添加好友列表，每个申请有一组信息

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
        List<BlackItem> ListBlack { get; set; }// 黑名单列表，每个有一组信息

        // 好友消息列表，每个好友一组消息列表，SingleChatMsg
    }
}