#if DEF_CLIENT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DEF.IM
{
    public partial class ComIMPlayerRegion : IComponentObserverIMPlayerRegion
    {
        public System.Action<RegionChatMsg> ActionOnRecvRegionChatMsg { get; set; }
        IComponentHook<ComIMPlayerRegion> Hook { get; set; }

        void AwakeClient(Dictionary<string, object> create_params)
        {
            Debug.Log($"ComIMPlayerRegion.AwakeClient() RegionGuid={State.RegionGuid} RegionId={State.RegionId}");

            var factory = Scene.Client.TryGetComponentHookFactory<ComIMPlayerRegion>();
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

            Debug.Log($"ComIMPlayerRegion.OnDestroyClient()");
        }

        void HandleEventClient(DEF.Event ev)
        {
            Hook?.HandleEvent(ev);
        }

        // 测试
        Task IComponentObserverIMPlayerRegion.Test(string s)
        {
            return Task.CompletedTask;
        }

        // 收到分区消息
        Task IComponentObserverIMPlayerRegion.OnRecvRegionChatMsg(RegionChatMsg msg)
        {
            ActionOnRecvRegionChatMsg?.Invoke(msg);

            return Task.CompletedTask;
        }

        public Task SendRegionChatMsg(RegionChatMsg msg)
        {
            var rpc = GetEntityRpc<IComponentRpcIMPlayerRegion>();
            return rpc.RequestSendRegionChatMsg(msg);
        }
    }
}

#endif