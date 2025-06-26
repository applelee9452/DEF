#if !DEF_CLIENT

using MemoryPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

// 单实例，一个IM玩家只属于一个分区
public partial class ComIMPlayerRegion : IComponentRpcIMPlayerRegion
{
    ComIMPlayer ComPlayer { get; set; }
    ContainerStatefulStreamSub<SStreamInfo> StreamSubRegion { get; set; }

    void AwakeServer(Dictionary<string, object> create_params)
    {
        Entity.Export4Bson = true;
        Entity.ExportEntityData = true;
        Entity.SetNetworkSyncFlag(true);

        ComPlayer = Scene.GetComponentFromBlackboard<ComIMPlayer>();

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

    // 请求发送区域消息
    Task IComponentRpcIMPlayerRegion.RequestSendRegionChatMsg(RegionChatMsg msg)
    {
        var c = GetContainerRpc<IContainerStatefulIMRegion>(State.RegionGuid);
        return c.SendRegionChatMsg(msg);
    }

    // 赋予该玩家分区
    public async Task AssignRegion(ContainerStatefulIMPlayer c, Region region)
    {
        await UnSubRegion();

        State.RegionGuid = region.RegionGuid;
        State.RegionId = region.RegionId;
        State.RegionDt = region.Dt;

        await SubRegion(c);
    }

    // 订阅区域
    public async Task SubRegion(ContainerStatefulIMPlayer c)
    {
        if (StreamSubRegion == null)
        {
            StreamSubRegion = await c.CreateStreamSubAsync<SStreamInfo>(
                StringDef.StreamNameSpaceRegion, State.RegionGuid, OnStreamRegion);
        }
    }

    // 取消订阅区域
    public async Task UnSubRegion()
    {
        if (StreamSubRegion != null)
        {
            await StreamSubRegion.UnsubAsync();
            StreamSubRegion = null;
        }
    }

    // 区域流
    Task OnStreamRegion(SStreamInfo s, Orleans.Streams.StreamSequenceToken token = null)
    {
        if (s.Id == SStreamId.RegionChatMsg)
        {
            if (ComPlayer != null && !string.IsNullOrEmpty(ComPlayer.GatewayGuid) && !string.IsNullOrEmpty(ComPlayer.SessionGuid))
            {
                var msg = MemoryPackSerializer.Deserialize<RegionChatMsg>(s.Data);

                var ob = GetEntityRpcObserver<IComponentObserverIMPlayerRegion>(ComPlayer.GatewayGuid, ComPlayer.SessionGuid);
                ob.OnRecvRegionChatMsg(msg);
            }
        }
        else if (s.Id == SStreamId.RegionSystemMail)
        {
            var system_mail = MemoryPackSerializer.Deserialize<SystemMail>(s.Data);

            var com_im_player_mailbox = Scene.GetComponentFromBlackboard<ComIMPlayerMailBox>();
            com_im_player_mailbox.AddMailFromSystemMail(system_mail);
        }

        return Task.CompletedTask;
    }
}

#endif