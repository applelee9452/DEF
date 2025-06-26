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
    public partial class ComIMGroup : IComponentObserverIMGroup
    {
        IComponentHook<ComIMGroup> Hook { get; set; }

        void AwakeClient(Dictionary<string, object> create_params)
        {
            Debug.Log($"ComIMGroup.AwakeClient() State.GroupGuid={State.GroupGuid}");

            //Context.ScenePlayer = Scene;
            //Context.ComPlayer = this;
            //Context.PlayerGuid = Scene.ContainerId;

            //ListenEvent<EventApplicationFocus>();

            //State.OnNickNameChanged = (s) =>
            //{
            //    var ev = Entity.GenEvent<EventEntityPlayerNickNameChanged>();
            //    ev.NickName = s;
            //    ev.Broadcast();
            //};

            //State.OnGoldChanged = (s) =>
            //{
            //    var ev = Entity.GenEvent<EventEntityPlayerGoldChanged>();
            //    ev.Gold = State.Gold;
            //    ev.Broadcast();
            //};

            //State.OnExpChanged = (e) =>
            //{
            //    var ev = Entity.GenEvent<EventExpChanged>();
            //    ev.Exp = State.Exp;
            //    ev.Broadcast();
            //};

            var factory = Scene.Client.TryGetComponentHookFactory<ComIMGroup>();
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
            //Context.ScenePlayer = null;
            //Context.ComPlayer = null;
            //Context.PlayerGuid = string.Empty;

            Hook?.OnDestroy(reason, user_data);

            Hook = null;

            Debug.Log($"ComIMGroup.OnDestroyClient() State.GroupGuid={State.GroupGuid}");
        }

        void HandleEventClient(DEF.Event ev)
        {
            Hook?.HandleEvent(ev);
        }

        // 测试
        Task IComponentObserverIMGroup.Test(string s)
        {
            Debug.Log($"ComIMGroup.Test() s={s}");

            return Task.CompletedTask;
        }
    }
}

#endif