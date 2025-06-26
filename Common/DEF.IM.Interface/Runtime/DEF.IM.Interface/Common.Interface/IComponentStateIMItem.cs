#if !DEF_CLIENT
using Orleans;
#endif
using DEF;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    [Component("IMItem")]
    public partial interface IComponentStateIMItem : IComponentState
    {
        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        int ItemId { get; set; }// 通过ItemId关联该Item的配置数据

        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.No, "")]
        string ItemObjId { get; set; }// Item实例Id
    }
}