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
    public partial class ComIMPlayerFriends : IComponentObserverIMPlayerFriends
    {
        IComponentHook<ComIMPlayerFriends> Hook { get; set; }

        void AwakeClient(Dictionary<string, object> create_params)
        {
            Debug.Log($"ComIMPlayerFriends.AwakeClient()");

            var factory = Scene.Client.TryGetComponentHookFactory<ComIMPlayerFriends>();
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

            Debug.Log($"ComIMPlayerFriends.OnDestroyClient()");
        }

        void HandleEventClient(DEF.Event ev)
        {
            Hook?.HandleEvent(ev);
        }

        // 收到被添加好友消息
        Task IComponentObserverIMPlayerFriends.OnAddFriend(FriendItem friend_item)
        {
            Debug.Log($"ComIMPlayerFriends.OnAddFriend() NickName={friend_item.NickName}");

            return Task.CompletedTask;
        }

        // 收到单聊消息
        Task IComponentObserverIMPlayerFriends.RecvSingleChatMsg(SingleChatMsgRecv msg)
        {
            Debug.Log($"ComIMPlayerFriends.RecvSingleChatMsg() Msg={msg.Msg}");

            return Task.CompletedTask;
        }
    }
}

#endif