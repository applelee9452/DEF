#if !DEF_CLIENT
using Orleans;
#endif
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    [Component("IMPlayerGroup")]
    public partial interface IComponentStateIMPlayerGroup : IComponentState
    {
        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        string GroupGuid { get; set; }// 所在群组Guid

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        string GroupName { get; set; }// 所在群组名
    }
}