#if !DEF_CLIENT

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MemoryPack;
using Microsoft.Extensions.Logging;

namespace DEF.IM;

// 区域，例如用于服务器分区
public class ContainerStatefulIMRegion : ContainerStateful, IContainerStatefulIMRegion
{
    Stopwatch StopwatchUpdate { get; set; }
    IDisposable TimerHandleUpdate { get; set; }
    ContainerStatefulStream<SStreamInfo> StreamRegion { get; set; }

    public override Task OnCreate()
    {
        Logger.LogDebug("ContainerStatefulIMRegion.OnCreate() Begin ContainerId={ContainerId}", ContainerId);

        //EtIMRegion = await Scene.CreateEntityFromDb(IMContext.Instance.Mongo.Database, StringDef.DbCollectionEntityGroup);

        //if (EtIMRegion == null)
        //{
        //    // 该区域不存在

        //    return;
        //}

        StreamRegion = CreateStream<SStreamInfo>(StringDef.StreamNameSpaceRegion, ContainerId);

        //ComIMGroup = EtIMRegion.GetComponent<ComIMGroup>();

        //ComIMGroup.OnCreate(StreamGroup);

        StopwatchUpdate = new Stopwatch();
        StopwatchUpdate.Start();
        TimerHandleUpdate = RegisterTimer((_) => TimerUpdate(),
            null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));

        Logger.LogDebug("ContainerStatefulIMRegion.OnCreate() End ContainerId={ContainerId}", ContainerId);

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
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

        Logger.LogDebug("ContainerStatefulIMRegion.OnDestroy() ContainerId={ContainerId}", ContainerId);

        return Task.CompletedTask;
    }

    // 保活
    Task IContainerStatefulIMRegion.Touch()
    {
        return Task.CompletedTask;
    }

    // 初始化，新建群时调用一次。群名，群主Guid
    //async Task<CreateRegionResult> IContainerStatefulIMRegion.Setup(string region_guid, int region_id)
    //{
    //    CreateRegionResult r = new()
    //    {
    //        Result = IMResult.Error,
    //        RegionGuid = string.Empty,
    //        RegionId = 0,
    //    };

    //    //if (EtIMRegion != null)
    //    //{
    //    //    r.Result = IMResult.IllegalRequest;
    //    //    return r;
    //    //}

    //    //if (EtIMRegion == null)
    //    //{
    //    //    StreamGroup = CreateStream<SStreamInfo>(StringDef.StreamNameSpaceGroup, ContainerId);

    //    //    EntityDef entity_def = new("IMGroup", typeof(ComIMGroup))
    //    //    {
    //    //        Children = [],
    //    //    };

    //    //    EtIMRegion = Scene.CreateEntity(entity_def);
    //    //    ComIMGroup = EtIMRegion.GetComponent<ComIMGroup>();

    //    //    ComIMGroup.OnSetup(group_name, admin, StreamGroup);

    //    //    // 获取新的自增PlayerId，用作昵称
    //    //    //{
    //    //    //    var filter = Builders<ConfigUniqId>.Filter
    //    //    //        .Where(x => x._id.Equals(StringDef.DocConfigUniqId));
    //    //    //    var update = Builders<ConfigUniqId>.Update
    //    //    //        .Inc<ulong>(x => x.UniqPlayerId, 1);

    //    //    //    var collection = Db.GetCollection<ConfigUniqId>(StringDef.DbCollectionConfigUniqId);

    //    //    //    FindOneAndUpdateOptions<ConfigUniqId> fuo = new() { IsUpsert = true };

    //    //    //    var doc = await collection.FindOneAndUpdateAsync(filter, update, fuo);

    //    //    //    ComIMGroup.State.ActorId = (long)doc.UniqPlayerId;
    //    //    //    ComIMGroup.State.IsBot = IsBot;
    //    //    //    ComIMGroup.State.BotId = BotId;
    //    //    //}

    //    //    await EtIMRegion.SyncAllStates2Db(IMContext.Instance.Mongo.Database, StringDef.DbCollectionEntityGroup);

    //    //    r.Result = IMResult.Success;
    //    //    r.GroupGuid = ComIMGroup.State.GroupGuid;
    //    //    r.GroupName = ComIMGroup.State.GroupName;

    //    //    return r;
    //    //}

    //    return r;
    //}

    // 发送群组消息
    Task IContainerStatefulIMRegion.SendRegionChatMsg(RegionChatMsg msg)
    {
        SStreamInfo s = new()
        {
            Id = SStreamId.RegionChatMsg,
            Data = MemoryPackSerializer.Serialize(msg)
        };
        return StreamRegion.OnNextAsync(s);
    }

    // 发送群组消息
    Task IContainerStatefulIMRegion.SendRegionSystemMail(SystemMail msg)
    {
        SStreamInfo s = new()
        {
            Id = SStreamId.RegionSystemMail,
            Data = MemoryPackSerializer.Serialize(msg)
        };
        return StreamRegion.OnNextAsync(s);
    }

    // 定时器更新
    Task TimerUpdate()
    {
        float tm = (float)StopwatchUpdate.Elapsed.TotalSeconds;
        StopwatchUpdate.Restart();

        return Task.CompletedTask;
    }
}

#endif