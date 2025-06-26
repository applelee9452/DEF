#if DEF_CLIENT

using DEF.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace DEF.IM
{
    public partial class ComIMPlayerMailBox : IComponentObserverIMPlayerMailBox
    {
        IComponentHook<ComIMPlayerMailBox> Hook { get; set; }

        void AwakeClient(Dictionary<string, object> create_params)
        {
            Debug.Log($"ComIMPlayerMailBox.AwakeClient()");

            var factory = Scene.Client.TryGetComponentHookFactory<ComIMPlayerMailBox>();
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

            Debug.Log($"ComIMPlayerMailBox.OnDestroyClient()");
        }

        void HandleEventClient(DEF.Event ev)
        {
            Hook?.HandleEvent(ev);
        }

        // 收到邮件列表
        Task IComponentObserverIMPlayerMailBox.RecvMailList(List<Mail> list_mail)
        {
            Debug.Log($"ComIMPlayerMailBox.RecvMailList()");
            return Task.CompletedTask;
        }

        public Task<int> RequestReadMails(string mail_guid)
        {
            var rpc = GetEntityRpc<IComponentRpcIMPlayerMailBox>();
            return rpc.RequestReadMails(mail_guid);
        }

        public Task<List<Mail>> RequestGetReward(string mail_guid)
        {
            var rpc = GetEntityRpc<IComponentRpcIMPlayerMailBox>();
            return rpc.RequestGetReward(mail_guid);
        }

        public Task<int> RequestDeleteMails()
        {
            var rpc = GetEntityRpc<IComponentRpcIMPlayerMailBox>();
            return rpc.RequestDeleteMails();
        }
    }
}

#endif