#if !DEF_CLIENT
using Microsoft.Extensions.Logging;
#endif
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DEF.IM
{
    // 群组组件
    [DEF.ComponentImpl]
    public partial class ComIMGroup : Component<IComponentStateIMGroup>
    {
        public override void Awake(Dictionary<string, object> create_params)
        {
            Scene.Add2Blackboard(this);

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
            UnListenAllEvent();

#if !DEF_CLIENT
            OnDestroyServer(reason, user_data);
#else
            OnDestroyClient(reason, user_data);
#endif

            Scene.RemoveFromBlackboard(this);
        }

        public override void HandleSelfEvent(DEF.SelfEvent ev)
        {
        }

        public override void HandleEvent(Event ev)
        {
#if !DEF_CLIENT
            HandleEventServer(ev);
#else
            HandleEventClient(ev);
#endif
        }
    }
}