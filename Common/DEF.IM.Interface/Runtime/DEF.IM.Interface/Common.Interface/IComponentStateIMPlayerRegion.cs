#if !DEF_CLIENT
using Orleans;
#endif
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    [Component("IMPlayerRegion")]
    public partial interface IComponentStateIMPlayerRegion : IComponentState
    {
        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        string RegionGuid { get; set; }// 所在分区Guid

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        int RegionId { get; set; }// 所在分区Id

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        DateTime RegionDt { get; set; }// 所在分区创建时间
    }
}