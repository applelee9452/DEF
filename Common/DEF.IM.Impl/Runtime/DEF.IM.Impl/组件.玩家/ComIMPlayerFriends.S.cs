#if !DEF_CLIENT

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

public partial class ComIMPlayerFriends : IComponentRpcIMPlayerFriends
{
    ComIMPlayer ComPlayer { get; set; }

    void AwakeServer(Dictionary<string, object> create_params)
    {
        Entity.Export4Bson = true;
        Entity.ExportEntityData = true;
        Entity.SetNetworkSyncFlag(true);

        Scene.Add2Blackboard(this);

        ComPlayer = Scene.GetComponentFromBlackboard<ComIMPlayer>();

        State.ListFriend ??= [];
        State.ListAddFriend ??= [];
        State.ListBlack ??= [];
    }

    void OnStartServer()
    {
    }

    void OnDestroyServer(string reason, byte[] user_data)
    {
        Scene.RemoveFromBlackboard(this);
    }

    void HandleEventServer(DEF.Event ev)
    {
    }

    // 请求添加指定玩家Id为好友，这是第1步，C2S
    Task IComponentRpcIMPlayerFriends.RequestAddFriend(string player_guid)
    {
        if (string.IsNullOrEmpty(player_guid))
        {
            // player_guid不合法
            return Task.CompletedTask;
        }

        if (player_guid == ComPlayer.State.PlayerGuid)
        {
            // 待添加的player_guid是本人，不可以添加本人为好友
            return Task.CompletedTask;
        }

        bool exist_in_listfriend = State.ListFriend.Exists((item) =>
        {
            if (item.PlayerGuid == player_guid)
            {
                return true;
            }

            return false;
        });

        if (exist_in_listfriend)
        {
            // 已经是我的好友，不需要重复添加
            return Task.CompletedTask;
        }

        //bool exist_in_listaddfriend = State.ListAddFriend.Exists((item) =>
        //{
        //    if (item.PlayerGuid == player_guid)
        //    {
        //        return true;
        //    }

        //    return false;
        //});

        //if (exist_in_listaddfriend)
        //{
        //    // 已经在我的申请好友列表，不需要重复申请
        //    return Task.CompletedTask;
        //}

        bool exist_in_listblack = State.ListBlack.Exists((item) =>
        {
            if (item.PlayerGuid == player_guid)
            {
                return true;
            }

            return false;
        });

        if (exist_in_listblack)
        {
            // 已经在我的黑名单中，不可以添加好友
            return Task.CompletedTask;
        }

        // 把我加到对方的申请好友列表中
        AddFriendItem add_friend = new()
        {
            PlayerGuid = ComPlayer.State.PlayerGuid,
            NickName = ComPlayer.State.NickName,
            Icon = ComPlayer.State.Icon,
        };

        var c = GetContainerRpc<IContainerStatefulIMPlayer>(player_guid);
        return c.Add2AddFriendList(add_friend);
    }

    // 响应对方的添加好友请求，deal_mode，忽略=0，同意=1，拒绝=2
    Task IComponentRpcIMPlayerFriends.ResponseAddFriend(string player_guid, int deal_mode)
    {
        if (string.IsNullOrEmpty(player_guid))
        {
            // player_guid不合法
            return Task.CompletedTask;
        }

        if (player_guid == ComPlayer.State.PlayerGuid)
        {
            // 待添加的player_guid是本人，不可以添加本人为好友
            return Task.CompletedTask;
        }

        AddFriendItem add_friend = null;

        bool exist = State.ListAddFriend.Exists((item) =>
        {
            if (item.PlayerGuid == player_guid)
            {
                add_friend = item;

                return true;
            }

            return false;
        });

        if (!exist)
        {
            // 待添加player_guid不在在我的申请好友列表中，无法处理
            return Task.CompletedTask;
        }

        // 从我的申请好友列表中移除
        State.ListAddFriend.RemoveAll((item) =>
        {
            if (item.PlayerGuid == player_guid)
            {
                return true;
            }

            return false;
        });

        switch (deal_mode)
        {
            case 0:
                {
                    // 忽略，上方从申请好友列表中清空后，什么也不需要做
                }
                break;
            case 1:
                {
                    // 同意，把对方添加到我的好友列表中，并告知对方，也把我添加为好友

                    if (add_friend != null)
                    {
                        FriendItem friend_item = new()
                        {
                            PlayerGuid = add_friend.PlayerGuid,
                            NickName = add_friend.NickName,
                            Icon = add_friend.Icon,
                        };

                        State.ListFriend.Add(friend_item);

                        var c = GetContainerRpc<IContainerStatefulIMPlayer>(friend_item.PlayerGuid);
                        return c.Add2AddFriendList(add_friend);
                    }
                }
                break;
            case 2:
                {
                    // 拒绝，告知对方
                }
                break;
        }

        return Task.CompletedTask;
    }

    // 批量响应对方的添加好友请求，deal_mode，忽略=0，同意=1，拒绝=2
    Task IComponentRpcIMPlayerFriends.ResponseAddFriends(List<string> list_player_guid, int deal_mode)
    {
        if (list_player_guid == null || list_player_guid.Count == 0)
        {
            return Task.CompletedTask;
        }

        List<AddFriendItem> list_add_friend = [];

        foreach (var player_guid in list_player_guid)
        {
            if (string.IsNullOrEmpty(player_guid))
            {
                // player_guid不合法
                continue;
            }

            if (player_guid == ComPlayer.State.PlayerGuid)
            {
                // 待添加的player_guid是本人，不可以添加本人为好友
                continue;
            }

            AddFriendItem add_friend = null;

            bool exist = State.ListAddFriend.Exists((item) =>
            {
                if (item.PlayerGuid == player_guid)
                {
                    add_friend = item;

                    return true;
                }

                return false;
            });

            if (!exist)
            {
                // 待添加player_guid不在在我的申请好友列表中，无法处理
                continue;
            }

            // 从我的申请好友列表中移除
            State.ListAddFriend.RemoveAll((item) =>
            {
                if (item.PlayerGuid == player_guid)
                {
                    return true;
                }

                return false;
            });

            switch (deal_mode)
            {
                case 0:
                    {
                        // 忽略，上方从申请好友列表中清空后，什么也不需要做
                    }
                    break;
                case 1:
                    {
                        // 同意，把对方添加到我的好友列表中，并告知对方，也把我添加为好友

                        if (add_friend != null)
                        {
                            list_add_friend.Add(add_friend);
                        }
                    }
                    break;
                case 2:
                    {
                        // 拒绝，告知对方
                    }
                    break;
            }
        }

        List<Task> list_task = [];
        foreach (var add_friend in list_add_friend)
        {
            FriendItem friend_item = new()
            {
                PlayerGuid = add_friend.PlayerGuid,
                NickName = add_friend.NickName,
                Icon = add_friend.Icon,
            };

            State.ListFriend.Add(friend_item);

            var c = GetContainerRpc<IContainerStatefulIMPlayer>(friend_item.PlayerGuid);
            var t = c.Add2AddFriendList(add_friend);
            list_task.Add(t);
        }

        return Task.WhenAll(list_task);
    }

    // 请求删除好友。把player_guid从我的好友列表中删除，并让对方我把从对方的好友列表中删除
    Task IComponentRpcIMPlayerFriends.RequestDeleteFriend(string player_guid)
    {
        if (string.IsNullOrEmpty(player_guid))
        {
            // player_guid不合法
            return Task.CompletedTask;
        }

        if (player_guid == ComPlayer.State.PlayerGuid)
        {
            // 待添加的player_guid是本人，不可以添加本人为好友
            return Task.CompletedTask;
        }

        // 把player_guid从我的好友列表中删除
        State.ListFriend.RemoveAll((item) =>
        {
            if (item.PlayerGuid == player_guid)
            {
                return true;
            }

            return false;
        });

        // 让对方我把从对方的好友列表中删除
        var c = GetContainerRpc<IContainerStatefulIMPlayer>(player_guid);
        return c.DeleteFriend(ComPlayer.State.PlayerGuid);
    }

    // 请求发送私聊消息
    Task IComponentRpcIMPlayerFriends.RequestSendSingleChatMsg(SingleChatMsgSend msg)
    {
        if (msg == null
            || string.IsNullOrEmpty(msg.SenderGuid)
            || string.IsNullOrEmpty(msg.RecverGuid)
            || string.IsNullOrEmpty(msg.Msg))
        {
            // 参数不合法
            return Task.CompletedTask;
        }

        if (ComPlayer.State.PlayerGuid != msg.SenderGuid)
        {
            // 发送者必须是本人
            return Task.CompletedTask;
        }

        // 单聊消息缓存容器
        string cid = ContainerStatefulIMSingleChatMsgCache.GenGrainKey(msg.SenderGuid, msg.RecverGuid);

        var c = GetContainerRpc<IContainerStatefulIMSingleChatMsgCache>(cid);
        return c.AddSingleChatMsg(msg);
    }

    // 请求获取最新的好友私聊消息
    Task<List<SingleChatMsgRecv>> IComponentRpcIMPlayerFriends.RequestGetLastestChatMsg(string player_guid)
    {
        string cid = ContainerStatefulIMSingleChatMsgCache.GenGrainKey(ComPlayer.State.PlayerGuid, player_guid);

        var c = GetContainerRpc<IContainerStatefulIMSingleChatMsgCache>(cid);
        return c.GetSingleChatMsgRecord();
    }

    // 添加好友，先放到我的申请好友列表中。第2步，S2S
    public Task Add2AddFriendList(AddFriendItem add_friend)
    {
        bool exist = State.ListAddFriend.Exists((item) =>
        {
            if (item.PlayerGuid == add_friend.PlayerGuid)
            {
                return true;
            }

            return false;
        });

        if (exist)
        {
            // 待添加player_guid已经在我的申请好友列表中，无需重复添加
            return Task.CompletedTask;
        }

        State.ListAddFriend.Add(add_friend);

        return Task.CompletedTask;
    }

    // 添加好友，添加好友列表中。第3步，S2S
    public Task Add2FriendList(FriendItem friend_item)
    {
        bool exist = State.ListFriend.Exists((item) =>
        {
            if (item.PlayerGuid == friend_item.PlayerGuid)
            {
                return true;
            }

            return false;
        });

        if (exist)
        {
            // 待添加player_guid已经在我的好友列表中，无需重复添加
            return Task.CompletedTask;
        }

        // 添加到好友列表中
        State.ListFriend.Add(friend_item);

        // 通知客户端
        if (ComPlayer != null && !string.IsNullOrEmpty(ComPlayer.GatewayGuid) && !string.IsNullOrEmpty(ComPlayer.SessionGuid))
        {
            var ob = GetEntityRpcObserver<IComponentObserverIMPlayerFriends>(ComPlayer.GatewayGuid, ComPlayer.SessionGuid);
            return ob.OnAddFriend(friend_item);
        }

        return Task.CompletedTask;
    }

    // 删除好友，从我的好友列表，申请列表中移除
    public Task DeleteFriend(string player_guid)
    {
        if (string.IsNullOrEmpty(player_guid))
        {
            // player_guid不合法
            return Task.CompletedTask;
        }

        // 把player_guid从我的好友列表中删除
        State.ListFriend.RemoveAll((item) =>
        {
            if (item.PlayerGuid == player_guid)
            {
                return true;
            }

            return false;
        });

        return Task.CompletedTask;
    }

    // 收到单聊消息，推送给Client
    public Task RecvSingleChatMsg(SingleChatMsgRecv msg)
    {
        // 通知客户端
        if (ComPlayer != null && !string.IsNullOrEmpty(ComPlayer.GatewayGuid) && !string.IsNullOrEmpty(ComPlayer.SessionGuid))
        {
            var ob = GetEntityRpcObserver<IComponentObserverIMPlayerFriends>(ComPlayer.GatewayGuid, ComPlayer.SessionGuid);
            return ob.RecvSingleChatMsg(msg);
        }

        return Task.CompletedTask;
    }
}

#endif