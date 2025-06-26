#if DEF_CLIENT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DEF.IM
{
    public partial class ComIMPlayerGroup : IComponentObserverIMPlayerGroup
    {
        public System.Action<GroupChatMsg> ActionOnRecvGroupChatMsg { get; set; }
        IComponentHook<ComIMPlayerGroup> Hook { get; set; }

        void AwakeClient(Dictionary<string, object> create_params)
        {
            Debug.Log($"ComIMPlayerGroup.AwakeClient() GroupGuid={State.GroupGuid} GroupName={State.GroupName}");

            var factory = Scene.Client.TryGetComponentHookFactory<ComIMPlayerGroup>();
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

            Debug.Log($"ComIMPlayerGroup.OnDestroyClient()");
        }

        void HandleEventClient(DEF.Event ev)
        {
            Hook?.HandleEvent(ev);
        }

        // 测试
        Task IComponentObserverIMPlayerGroup.Test(string s)
        {
            return Task.CompletedTask;
        }

        // 收到群聊消息
        Task IComponentObserverIMPlayerGroup.OnRecvGroupChatMsg(GroupChatMsg msg)
        {
            ActionOnRecvGroupChatMsg?.Invoke(msg);

            return Task.CompletedTask;
        }

        // 请求解散该群组
        public Task RequestDisbandGroup()
        {
            var rpc = GetEntityRpc<IComponentRpcIMPlayerGroup>();
            return rpc.RequestDisbandGroup();
        }

        // 请求退出该群组
        public Task RequestLeaveGroup(string new_admin_guid)
        {
            var rpc = GetEntityRpc<IComponentRpcIMPlayerGroup>();
            return rpc.RequestLeaveGroup(new_admin_guid);
        }

        // 请求发送群组消息
        public Task RequestSendGroupChatMsg(GroupChatMsg msg)
        {
            var rpc = GetEntityRpc<IComponentRpcIMPlayerGroup>();
            return rpc.RequestSendGroupChatMsg(msg);
        }
    }
}

#endif