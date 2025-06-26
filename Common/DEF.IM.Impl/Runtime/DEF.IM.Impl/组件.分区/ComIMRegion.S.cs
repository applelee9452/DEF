#if !DEF_CLIENT

using MemoryPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DEF.IM;

// 多实例，不可以加到Scene黑板中
public partial class ComIMRegion : IComponentRpcIMRegion
{
    public Random Rd { get; set; } = new(Guid.NewGuid().GetHashCode());
    public string GatewayGuid { get; private set; } = string.Empty;
    public string SessionGuid { get; private set; } = string.Empty;
    public bool ClientOnline { get; private set; } = false;// Client是否在线
    ContainerStatefulStream<SStreamInfo> StreamGroup { get; set; }
    Stopwatch StopwatchOnline { get; set; }// 玩家在线时长秒数计算

    void AwakeServer(Dictionary<string, object> create_params)
    {
        Entity.Export4Bson = true;
        Entity.ExportEntityData = true;
        Entity.SetNetworkSyncFlag(true);

        StopwatchOnline = new Stopwatch();

        Scene.Add2Blackboard(this);
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

    // 请求修改群名
    //Task IComponentRpcIMRegion.ModifyGroupName(string group_name)
    //{
    //    State.GroupName = group_name;

    //    return Task.CompletedTask;
    //}

    // 群初始化
    public void OnSetup(string group_name, GroupMember admin, ContainerStatefulStream<SStreamInfo> stream)
    {
        //State.GroupGuid = Scene.ContainerId;
        //State.GroupName = group_name;
        //State.AdminGuid = admin.PlayerGuid;

        //State.MapMember = new()
        //{
        //    { admin.PlayerGuid, admin }
        //};

        StreamGroup = stream;
    }

    // 群加载
    public void OnCreate(ContainerStatefulStream<SStreamInfo> stream)
    {
        StreamGroup = stream;
    }

    // 发送群组消息
    public Task SendGroupChatMsg(GroupChatMsg msg)
    {
        SStreamInfo s = new()
        {
            Id = SStreamId.GroupChatMsg,
            Data = MemoryPackSerializer.Serialize(msg)
        };

        return StreamGroup.OnNextAsync(s);
    }

    // 请求解散该群组
    public Task<IMResult> RequestDisbandGroup(string player_guid)
    {
        IMResult r = IMResult.Error;

        //if (State.AdminGuid != player_guid)
        //{
        //    r = IMResult.NoPermission;// 权限不足

        //    return Task.FromResult(r);
        //}

        //State.AdminGuid = string.Empty;
        //State.MapMember = null;
        r = IMResult.Success;

        return Task.FromResult(r);
    }

    // 服务端Tick
    public async Task UpdateServer(float tm)
    {
        await Scene.SyncDelta2Client();

        //if (!string.IsNullOrEmpty(SessionGuid))
        //{
        //    WriteDbTm += tm;
        //    if (WriteDbTm > 60f)
        //    {
        //        WriteDbTm = 0;
        //        await Entity.SyncAllStates2Db(MaoContext.Instance.Mongo, StringDef.DbCollectionEntityPlayer);
        //    }
        //}
    }
}

#endif