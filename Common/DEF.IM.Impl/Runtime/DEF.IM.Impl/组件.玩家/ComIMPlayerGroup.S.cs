#if !DEF_CLIENT

using MemoryPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

public partial class ComIMPlayerGroup : IComponentRpcIMPlayerGroup
{
    ComIMPlayer ComPlayer { get; set; }
    ContainerStatefulStreamSub<SStreamInfo> StreamSubGroup { get; set; }

    void AwakeServer(Dictionary<string, object> create_params)
    {
        Entity.Export4Bson = true;
        Entity.ExportEntityData = true;
        Entity.SetNetworkSyncFlag(true);

        ComPlayer = Scene.GetComponentFromBlackboard<ComIMPlayer>();
        ComPlayer.MapIMPlayerGroup ??= [];

        if (create_params != null)
        {
            var r = (CreateGroupResult)create_params["R"];
            State.GroupGuid = r.GroupGuid;
            State.GroupName = r.GroupName;
        }

        ComPlayer.MapIMPlayerGroup[State.GroupGuid] = this;
    }

    void OnStartServer()
    {
    }

    void OnDestroyServer(string reason, byte[] user_data)
    {
        ComPlayer.MapIMPlayerGroup.Remove(State.GroupGuid);
    }

    void HandleEventServer(DEF.Event ev)
    {
    }

    // 请求解散该群组
    async Task IComponentRpcIMPlayerGroup.RequestDisbandGroup()
    {
        // 解散群组规则，解散者必须是群组

        var c = GetContainerRpc<IContainerStatefulIMGroup>(State.GroupGuid);
        var r = await c.RequestDisbandGroup(ComPlayer.State.PlayerGuid);

        if (r == IMResult.Success)
        {
            await UnSubGroup();

            Entity.Destroy();
        }
    }

    // 请求退出该群组
    async Task IComponentRpcIMPlayerGroup.RequestLeaveGroup(string new_admin_guid)
    {
        // 退出群组规则

        var c = GetContainerRpc<IContainerStatefulIMGroup>(State.GroupGuid);
        var r = await c.RequestLeaveGroup(ComPlayer.State.PlayerGuid, new_admin_guid);

        if (r == IMResult.Success)
        {
            await UnSubGroup();

            Entity.Destroy();
        }
    }

    // 请求发送群消息
    Task IComponentRpcIMPlayerGroup.RequestSendGroupChatMsg(GroupChatMsg msg)
    {
        // 发送群消息规则

        msg.GroupGuid = State.GroupGuid;
        msg.SenderGuid = ComPlayer.State.PlayerGuid;
        msg.Dt = DateTime.UtcNow;

        var c = GetContainerRpc<IContainerStatefulIMGroup>(State.GroupGuid);
        return c.SendGroupChatMsg(msg);
    }

    // 订阅群组
    public async Task SubGroup(ContainerStatefulIMPlayer c)
    {
        StreamSubGroup = await c.CreateStreamSubAsync<SStreamInfo>(
            StringDef.StreamNameSpaceGroup, State.GroupGuid, OnStreamGroup);
    }

    // 取消订阅群组
    public async Task UnSubGroup()
    {
        if (StreamSubGroup != null)
        {
            await StreamSubGroup.UnsubAsync();
            StreamSubGroup = null;
        }
    }

    // 群组流
    Task OnStreamGroup(SStreamInfo s, Orleans.Streams.StreamSequenceToken token = null)
    {
        if (s.Id == SStreamId.GroupChatMsg)
        {
            if (ComPlayer != null && !string.IsNullOrEmpty(ComPlayer.GatewayGuid) && !string.IsNullOrEmpty(ComPlayer.SessionGuid))
            {
                var msg = MemoryPackSerializer.Deserialize<GroupChatMsg>(s.Data);

                var ob = GetEntityRpcObserver<IComponentObserverIMPlayerGroup>(ComPlayer.GatewayGuid, ComPlayer.SessionGuid);
                return ob.OnRecvGroupChatMsg(msg);
            }
        }

        return Task.CompletedTask;
    }
}

#endif