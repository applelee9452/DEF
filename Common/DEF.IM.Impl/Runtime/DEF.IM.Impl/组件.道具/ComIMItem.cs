using System;
using System.Collections;
using System.Collections.Generic;

namespace DEF.IM
{
    // 道具组件
    [DEF.ComponentImpl]
    public partial class ComIMItem : DEF.Component<IComponentStateIMItem>
    {
        public override void Awake(Dictionary<string, object> create_params)
        {
            //Entity.ExportFlag = ExportFlag.None;

#if !DEF_CLIENT
            AwakeServer(create_params);
#else
            AwakeClient(create_params);
#endif
        }

        public override void OnStart()
        {
#if !DEF_CLIENT
            OnStartServer();
#else
            OnStartClient();
#endif
        }

        public override void OnDestroy(string reason = null, byte[] user_data = null)
        {
#if !DEF_CLIENT
            OnDestroyServer(reason, user_data);
#else
            OnDestroyClient();
#endif
        }

        public override void HandleSelfEvent(SelfEvent ev)
        {
        }

        public override void HandleEvent(DEF.Event ev)
        {
#if !DEF_CLIENT
            HandleEventServer(ev);
#else
            HandleEventClient(ev);
#endif
        }
    }
}