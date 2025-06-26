using DEF;
using DEF.IM;
using System.Collections;
using System.Collections.Generic;

namespace DanMu
{
    // 装备组件
    [DEF.ComponentImpl]
    public partial class ComIMItemCDKey : DEF.Component<IComponentStateIMItemCDKey>
    {
        public ComIMItem ComItem { get; private set; }
        //public ComPlayerInventory ComPlayerInventory { get; private set; }// 可为空，如果为空则表示该道具不属于背包，而是属于箱子

        public override void Awake(Dictionary<string, object> create_params)
        {
            ComItem = Entity.GetComponent<ComIMItem>();
            //ComPlayerInventory = Entity.Parent.GetComponent<ComPlayerInventory>();

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