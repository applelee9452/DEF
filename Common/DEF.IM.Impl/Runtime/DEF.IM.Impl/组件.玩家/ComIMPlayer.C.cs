#if DEF_CLIENT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DEF.IM
{
    public class EventEntityIMPlayerAwake : DEF.Event
    {
        public ComIMPlayer ComPlayer { get; set; }
    }

    public class EventEntityIMPlayerOnDestroy : DEF.Event
    {
    }

    public partial class ComIMPlayer : IComponentObserverIMPlayer
    {
        IComponentHook<ComIMPlayer> Hook { get; set; }
        public System.Action<BIMMarquee> ActionOnRecvMarquee { get; set; }

        void AwakeClient(Dictionary<string, object> create_params)
        {
            Debug.Log($"ComPlayer.AwakeClient() State.PlayerGuid={State.PlayerGuid}");

            var ev = Entity.GenEvent<EventEntityIMPlayerAwake>();
            ev.ComPlayer = this;
            ev.Broadcast();

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

            var factory = Scene.Client.TryGetComponentHookFactory<ComIMPlayer>();
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
            var ev = Entity.GenEvent<EventEntityIMPlayerOnDestroy>();
            ev.Broadcast();

            Hook?.OnDestroy(reason, user_data);

            Hook = null;
            ActionOnRecvMarquee = null;

            Debug.Log($"ComPlayer.OnDestroyClient() State.PlayerGuid={State.PlayerGuid}");
        }

        void HandleEventClient(DEF.Event ev)
        {
            Hook?.HandleEvent(ev);
        }

        // 测试
        Task IComponentObserverIMPlayer.Test(string s)
        {
            //Debug.Log($"ComPlayer.Test() s={s}");

            return Task.CompletedTask;
        }

        // 收到跑马灯消息
        Task IComponentObserverIMPlayer.RecvMarquee(BIMMarquee marquee)
        {
            //Debug.Log($"ComPlayer.RecvMarquee() Msg={marquee.Msg}");

            ActionOnRecvMarquee?.Invoke(marquee);

            return Task.CompletedTask;
        }

        // 请求修改昵称
        public Task RequestChangeNickname(string nickname)
        {
            var rpc = GetEntityRpc<IComponentRpcIMPlayer>();
            return rpc.RequestChangeNickname(nickname);
        }

        // 请求创建群组
        public Task<CreateGroupResult> RequestCreateGroup(string group_name)
        {
            var rpc = GetEntityRpc<IComponentRpcIMPlayer>();
            return rpc.RequestCreateGroup(group_name);
        }

        // 请求加入群组
        public Task RequestJoinGroup(string group_guid)
        {
            var rpc = GetEntityRpc<IComponentRpcIMPlayer>();
            return rpc.RequestJoinGroup(group_guid);
        }
    }
}

#endif