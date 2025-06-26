#if !DEF_CLIENT

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DEF.IM;

// 群组
public class ContainerStatefulIMGroup : ContainerStateful, IContainerStatefulIMGroup
{
    Entity EtIMGroup { get; set; }
    ComIMGroup ComIMGroup { get; set; }
    Stopwatch StopwatchUpdate { get; set; }
    IDisposable TimerHandleUpdate { get; set; }
    ContainerStatefulStream<SStreamInfo> StreamGroup { get; set; }

    public override async Task OnCreate()
    {
        Logger.LogDebug("ContainerStatefulGroup.OnCreate() Begin ContainerId={ContainerId}", ContainerId);

        EtIMGroup = await Scene.CreateEntityFromDb(IMContext.Instance.Mongo.Database, StringDef.DbCollectionEntityGroup);

        if (EtIMGroup == null)
        {
            // 该群组不存在

            return;
        }

        StreamGroup = CreateStream<SStreamInfo>(StringDef.StreamNameSpaceGroup, ContainerId);

        ComIMGroup = EtIMGroup.GetComponent<ComIMGroup>();

        ComIMGroup.OnCreate(StreamGroup);

        StopwatchUpdate = new Stopwatch();
        StopwatchUpdate.Start();
        TimerHandleUpdate = RegisterTimer((_) => TimerUpdate(),
            null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));

        Logger.LogDebug("ContainerStatefulGroup.OnCreate() End ContainerId={ContainerId}", ContainerId);
    }

    public override async Task OnDestroy()
    {
        ComIMGroup = null;

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

        if (EtIMGroup != null)
        {
            await EtIMGroup.SyncDelta2Db(IMContext.Instance.Mongo.Database, StringDef.DbCollectionEntityGroup);
            EtIMGroup = null;
        }

        Logger.LogDebug("ContainerStatefulGroup.OnDestroy() ContainerId={ContainerId}", ContainerId);
    }

    // 保活
    Task IContainerStatefulIMGroup.Touch()
    {
        return Task.CompletedTask;
    }

    // 初始化，新建群时调用一次。群名，群主Guid
    async Task<CreateGroupResult> IContainerStatefulIMGroup.Setup(string group_name, GroupMember admin)
    {
        CreateGroupResult r = new()
        {
            Result = IMResult.Error,
            GroupGuid = string.Empty,
            GroupName = string.Empty,
        };

        if (EtIMGroup != null)
        {
            r.Result = IMResult.IllegalRequest;
            return r;
        }

        if (EtIMGroup == null)
        {
            StreamGroup = CreateStream<SStreamInfo>(StringDef.StreamNameSpaceGroup, ContainerId);

            EntityDef entity_def = new("IMGroup", typeof(ComIMGroup))
            {
                Children = [],
            };

            EtIMGroup = Scene.CreateEntity(entity_def);
            ComIMGroup = EtIMGroup.GetComponent<ComIMGroup>();

            ComIMGroup.OnSetup(group_name, admin, StreamGroup);

            // 获取新的自增PlayerId，用作昵称
            //{
            //    var filter = Builders<ConfigUniqId>.Filter
            //        .Where(x => x._id.Equals(StringDef.DocConfigUniqId));
            //    var update = Builders<ConfigUniqId>.Update
            //        .Inc<ulong>(x => x.UniqPlayerId, 1);

            //    var collection = Db.GetCollection<ConfigUniqId>(StringDef.DbCollectionConfigUniqId);

            //    FindOneAndUpdateOptions<ConfigUniqId> fuo = new() { IsUpsert = true };

            //    var doc = await collection.FindOneAndUpdateAsync(filter, update, fuo);

            //    ComIMGroup.State.ActorId = (long)doc.UniqPlayerId;
            //    ComIMGroup.State.IsBot = IsBot;
            //    ComIMGroup.State.BotId = BotId;
            //}

            await EtIMGroup.SyncDelta2Db(IMContext.Instance.Mongo.Database, StringDef.DbCollectionEntityGroup);

            r.Result = IMResult.Success;
            r.GroupGuid = ComIMGroup.State.GroupGuid;
            r.GroupName = ComIMGroup.State.GroupName;

            return r;
        }

        return r;
    }

    // 发送群组消息
    Task IContainerStatefulIMGroup.SendGroupChatMsg(GroupChatMsg msg)
    {
        if (ComIMGroup == null)
        {
            return Task.CompletedTask;
        }

        return ComIMGroup.SendGroupChatMsg(msg);
    }

    // 请求解散该群组
    Task<IMResult> IContainerStatefulIMGroup.RequestDisbandGroup(string player_guid)
    {
        if (ComIMGroup == null)
        {
            return Task.FromResult(IMResult.IllegalRequest);
        }

        return ComIMGroup.RequestDisbandGroup(player_guid);
    }

    // 请求退出该群组
    Task<IMResult> IContainerStatefulIMGroup.RequestLeaveGroup(string player_guid, string new_admin_guid)
    {
        if (ComIMGroup == null)
        {
            return Task.FromResult(IMResult.IllegalRequest);
        }

        return ComIMGroup.RequestLeaveGroup(player_guid, new_admin_guid);
    }

    // 定时器更新
    Task TimerUpdate()
    {
        float tm = (float)StopwatchUpdate.Elapsed.TotalSeconds;
        StopwatchUpdate.Restart();

        return ComIMGroup?.UpdateServer(tm);
    }
}

#endif