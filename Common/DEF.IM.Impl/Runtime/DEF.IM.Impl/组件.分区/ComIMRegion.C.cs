#if DEF_CLIENT

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DEF.IM
{
    public partial class ComIMRegion : IComponentObserverIMRegion
    {
        IComponentHook<ComIMRegion> Hook { get; set; }

        void AwakeClient(Dictionary<string, object> create_params)
        {
            Debug.Log($"ComIMRegion.AwakeClient() RegionGuid={State.RegionGuid}");

            var factory = Scene.Client.TryGetComponentHookFactory<ComIMRegion>();
            if (factory != null)
            {
                Hook = factory.CreateHook(this);
            }

            Hook?.OnAwake(create_params);
        }

        void OnStartClient()
        {
            Hook?.OnStart();
        }

        void OnDestroyClient(string reason, byte[] user_data)
        {
            Hook?.OnDestroy(reason, user_data);

            Hook = null;

            Debug.Log($"ComIMRegion.OnDestroyClient() RegionGuid={State.RegionGuid}");
        }

        void HandleEventClient(DEF.Event ev)
        {
            Hook?.HandleEvent(ev);
        }

        // 测试
        Task IComponentObserverIMRegion.Test(string s)
        {
            Debug.Log($"ComIMRegion.Test() s={s}");

            return Task.CompletedTask;
        }
    }
}

#endif