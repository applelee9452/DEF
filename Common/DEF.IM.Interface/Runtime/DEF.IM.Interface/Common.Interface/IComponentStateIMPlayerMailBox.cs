#if !DEF_CLIENT
using Orleans;
#endif
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    [Component("IMPlayerMailBox")]
    public partial interface IComponentStateIMPlayerMailBox : IComponentState
    {
        [Prop(PropType.Default, PropSyncFlag.SyncDbAndNetwork, PropSyncMode.Set, PropCallback.Yes, "")]
        List<Mail> ListMail { get; set; }// 邮件列表，有序的，最后一个是最新的邮件
    }
}