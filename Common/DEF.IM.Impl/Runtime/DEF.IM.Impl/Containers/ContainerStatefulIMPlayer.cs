#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MemoryPack;
using Microsoft.Extensions.Logging;

namespace DEF.IM;

// IM玩家
public class ContainerStatefulIMPlayer : ContainerStateful, IContainerStatefulIMPlayer
{
    Entity EtIMPlayer { get; set; }
    ComIMPlayer ComPlayer { get; set; }
    Stopwatch StopwatchUpdate { get; set; }
    IDisposable TimerHandleUpdate { get; set; }
    IDisposable TimerHandleSavePlayer { get; set; }
    ContainerStatefulStreamSub<SStreamInfo> StreamSubMarquee { get; set; }

    public override async Task OnCreate()
    {
        Logger.LogDebug("ContainerStatefullIMPlayer.OnCreate() PlayerGuid={ContainerId}", ContainerId);

        //if (int.TryParse(ContainerId, out int container_id_i))
        //{
        //    IsBot = true;
        //    BotId = container_id_i;
        //}

        EtIMPlayer = await Scene.CreateEntityFromDb(IMContext.Instance.Mongo.Database, StringDef.DbCollectionEntityPlayer);

        if (EtIMPlayer == null)
        {
            EntityDef entity_def = new("IMPlayer", typeof(ComIMPlayer))
            {
                Children =
                [
                    new("IMPlayerRegion", typeof(ComIMPlayerRegion))
                    {
                    },
                    new("IMPlayerFriends", typeof(ComIMPlayerFriends))
                    {
                    },
                    new("IMPlayerMailBox", typeof(ComIMPlayerMailBox))
                    {
                    }
                    // 群组是动态添加的，一开始没有群组
                ],
            };

            EtIMPlayer = Scene.CreateEntity(entity_def);
            ComPlayer = EtIMPlayer.GetComponent<ComIMPlayer>();

            await ComPlayer.OnNewPlayer(this);

            await EtIMPlayer.SyncDelta2Db(IMContext.Instance.Mongo.Database, StringDef.DbCollectionEntityPlayer);

            // 获取新的自增PlayerId，用作昵称
            //{
            //    var filter = Builders<ConfigUniqId>.Filter
            //        .Where(x => x._id.Equals(StringDef.DocConfigUniqId));
            //    var update = Builders<ConfigUniqId>.Update
            //        .Inc<ulong>(x => x.UniqPlayerId, 1);

            //    var collection = Db.GetCollection<ConfigUniqId>(StringDef.DbCollectionConfigUniqId);

            //    FindOneAndUpdateOptions<ConfigUniqId> fuo = new() { IsUpsert = true };

            //    var doc = await collection.FindOneAndUpdateAsync(filter, update, fuo);

            //    ComPlayer.State.ActorId = (long)doc.UniqPlayerId;
            //    ComPlayer.State.IsBot = IsBot;
            //    ComPlayer.State.BotId = BotId;
            //}

            //await EtPlayer.SyncAllStates2Db(MaoContext.Instance.Mongo, StringDef.DbCollectionEntityPlayer);
        }
        else
        {
            ComPlayer = EtIMPlayer.GetComponent<ComIMPlayer>();
        }

        await ComPlayer.OnCreate(this);

        //StreamSubServer = await CreateStreamSubAsync<StreamServer>(
        //    StringDef.StreamNameSpaceServer, StringDef.StreamGuidServer, OnStreamServer);

        // 订阅跑马灯流
        StreamSubMarquee = await CreateStreamSubAsync<SStreamInfo>(
            StringDef.StreamNameSpaceMarquee, StringDef.StreamGuidMarquee, OnStreamMarquee);

        // 订阅所有的群组流，0~n
        if (ComPlayer.MapIMPlayerGroup != null && ComPlayer.MapIMPlayerGroup.Count > 0)
        {
            List<Task> list_task = [];
            foreach (var i in ComPlayer.MapIMPlayerGroup)
            {
                var t = i.Value.SubGroup(this);
                list_task.Add(t);
            }
            await Task.WhenAll(list_task);
        }

        StopwatchUpdate = new Stopwatch();
        StopwatchUpdate.Start();
        TimerHandleUpdate = RegisterTimer((_) => TimerUpdate(),
            null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));

        TimerHandleSavePlayer = RegisterTimer((_) => TimerSaveEntityPlayer(),
            null, TimeSpan.FromMilliseconds(60000), TimeSpan.FromMilliseconds(60000));
    }

    public override async Task OnDestroy()
    {
        var com_implayer = ComPlayer;
        ComPlayer = null;

        if (TimerHandleUpdate != null)
        {
            TimerHandleUpdate.Dispose();
            TimerHandleUpdate = null;
        }

        if (StopwatchUpdate != null)
        {
            StopwatchUpdate.Stop();
            StopwatchUpdate = null;
        }

        if (TimerHandleSavePlayer != null)
        {
            TimerHandleSavePlayer.Dispose();
            TimerHandleSavePlayer = null;
        }

        if (StreamSubMarquee != null)
        {
            await StreamSubMarquee.UnsubAsync();
            StreamSubMarquee = null;
        }

        if (com_implayer is not null && com_implayer.MapIMPlayerGroup != null && com_implayer.MapIMPlayerGroup.Count > 0)
        {
            List<Task> list_task = [];
            foreach (var i in com_implayer.MapIMPlayerGroup)
            {
                var t = i.Value.UnSubGroup();
                list_task.Add(t);
            }
            await Task.WhenAll(list_task);
        }

        var com_implayerregion = Scene.GetComponentFromBlackboard<ComIMPlayerRegion>();
        await com_implayerregion.UnSubRegion();

        await EtIMPlayer.SyncDelta2Db(IMContext.Instance.Mongo.Database, StringDef.DbCollectionEntityPlayer);
        EtIMPlayer?.Destroy();
        EtIMPlayer = null;

        Logger.LogDebug("ContainerStatefullIMPlayer.OnDestroy() PlayerGuid={ContainerId}", ContainerId);
    }

    // 客户端连接并验证成功
    Task IContainerStatefulIMPlayer.ClientAttached(Gateway.GatewayAuthedInfo info, string extra_data)
    {
        if (ComPlayer == null)
        {
            return Task.CompletedTask;
        }

        return ComPlayer.ServerOnClientAttached(info, extra_data);
    }

    // 客户端断开链接
    Task IContainerStatefulIMPlayer.ClientDeattached(string session_guid)
    {
        if (ComPlayer == null)
        {
            return Task.CompletedTask;
        }

        return ComPlayer.ServerOnClientDeattached(session_guid, this);
    }

    // 添加好友，先放到申请好友列表中。第2步，S2S
    Task IContainerStatefulIMPlayer.Add2AddFriendList(AddFriendItem add_friend)
    {
        if (ComPlayer == null)
        {
            return Task.CompletedTask;
        }

        var com_friends = ComPlayer.Scene.GetComponentFromBlackboard<ComIMPlayerFriends>();
        return com_friends.Add2AddFriendList(add_friend);
    }

    // 添加好友，添加好友列表中。第3步，S2S
    Task IContainerStatefulIMPlayer.Add2FriendList(FriendItem friend_item)
    {
        if (ComPlayer == null)
        {
            return Task.CompletedTask;
        }

        var com_friends = ComPlayer.Scene.GetComponentFromBlackboard<ComIMPlayerFriends>();
        return com_friends.Add2FriendList(friend_item);
    }

    // 删除好友，从好友列表，申请列表中移除
    Task IContainerStatefulIMPlayer.DeleteFriend(string player_guid)
    {
        if (ComPlayer == null)
        {
            return Task.CompletedTask;
        }

        var com_friends = ComPlayer.Scene.GetComponentFromBlackboard<ComIMPlayerFriends>();
        return com_friends.DeleteFriend(player_guid);
    }

    // 收到单聊消息，推送给Client
    Task IContainerStatefulIMPlayer.RecvSingleChatMsg(SingleChatMsgRecv msg)
    {
        if (ComPlayer == null)
        {
            return Task.CompletedTask;
        }

        var com_friends = ComPlayer.Scene.GetComponentFromBlackboard<ComIMPlayerFriends>();
        return com_friends.RecvSingleChatMsg(msg);
    }

    // 请求获取玩家信息
    Task<PlayerInfo> IContainerStatefulIMPlayer.GetPlayerInfo()
    {
        if (ComPlayer == null)
        {
            return Task.FromResult((PlayerInfo)null);
        }

        var player_info = ComPlayer.GetPlayerInfo();

        return Task.FromResult(player_info);
    }

    // 请求获取初始化时玩家信息
    Task<PlayerInitInfo> IContainerStatefulIMPlayer.GetPlayerInitInfo()
    {
        if (ComPlayer == null)
        {
            return Task.FromResult((PlayerInitInfo)null);
        }

        var player_info = ComPlayer.GetPlayerInfo();
        var region_info = ComPlayer.GetRegionInfo();

        PlayerInitInfo player_init_info = new()
        {
            RegionGuid = region_info.RegionGuid,
            RegionId = region_info.RegionId,
            RegionDt = region_info.Dt,
            PlayerGuid = player_info.PlayerGuid,
            UId = player_info.UId
        };

        return Task.FromResult(player_init_info);
    }

    // 获取分区信息
    Task<Region> IContainerStatefulIMPlayer.GetRegionInfo()
    {
        var region_info = ComPlayer.GetRegionInfo();

        return Task.FromResult(region_info);
    }

    Task IContainerStatefulIMPlayer.AddMail(Mail mail)
    {
        if (ComPlayer == null)
        {
            return Task.CompletedTask;
        }

        var com_mailbox = ComPlayer.Scene.GetComponentFromBlackboard<ComIMPlayerMailBox>();
        com_mailbox.AddMail(mail);

        return Task.CompletedTask;
    }

    public Task SavePlayerEventData<T>(T player_event, string factory_name = "") where T : EventBase
    {
        string ev_name = typeof(T).Name;

        //Sb.Clear();
        //Sb.Append(ev_name);
        //DateTime now = DateTime.UtcNow;
        //Sb.Append('_');
        //Sb.Append(ContainerId);
        //Sb.Append('_');
        //Sb.Append(now.ToString("yyyy-MM-dd-HH-m-s"));
        //Sb.Append('_');
        //Sb.Append(++EventIndex);

        //string ev_key = Sb.ToString();
        player_event.EventTm = DateTime.UtcNow;
        player_event.EventType = ev_name;
        //player_event._id = ev_key;

        string collection_name = ev_name;
        if (!string.IsNullOrEmpty(factory_name))
        {
            collection_name += factory_name;
        }

        return IMContext.Instance.Mongo.InsertOneData(collection_name, player_event);
    }

    // 流广播消息，跑马灯广播
    Task OnStreamMarquee(SStreamInfo s, Orleans.Streams.StreamSequenceToken token = null)
    {
        if (ComPlayer == null)
        {
            return Task.CompletedTask;
        }

        if (s.Id == SStreamId.Marquee)
        {
            var im_marquee = MemoryPackSerializer.Deserialize<BIMMarquee>(s.Data);

            return ComPlayer.RecvMarquee(im_marquee);
        }

        return Task.CompletedTask;
    }

    // 定时器更新
    Task TimerUpdate()
    {
        float tm = (float)StopwatchUpdate.Elapsed.TotalSeconds;
        StopwatchUpdate.Restart();

        return ComPlayer?.UpdateServer(tm);
    }

    // 定时将玩家Entity保存数据库
    Task TimerSaveEntityPlayer()
    {
        return EtIMPlayer.SyncDelta2Db(IMContext.Instance.Mongo.Database, StringDef.DbCollectionEntityPlayer);
    }
}

#endif