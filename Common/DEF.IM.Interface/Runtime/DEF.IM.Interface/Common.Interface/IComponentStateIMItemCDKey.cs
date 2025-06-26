#if !DEF_CLIENT
using Orleans;
#endif
using DEF;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    [Component("IMItemCDKey")]
    public partial interface IComponentStateIMItemCDKey : IComponentState
    {
    }
}