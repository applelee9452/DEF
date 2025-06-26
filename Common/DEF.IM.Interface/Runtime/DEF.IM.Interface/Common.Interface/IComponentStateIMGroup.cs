#if !DEF_CLIENT
using Orleans;
#endif
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    [Component("IMGroup")]
    public partial interface IComponentStateIMGroup : IComponentState
    {
        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        string GroupGuid { get; set; }// 群组Guid

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        string GroupName { get; set; }// 群名

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        string GroupNotice { get; set; }// 群公告

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        string AdminGuid { get; set; }// 群主Guid

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        Dictionary<string, GroupMember> MapMember { get; set; }

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        bool IsDelete { get; set; }// 是否已经解散
    }
}