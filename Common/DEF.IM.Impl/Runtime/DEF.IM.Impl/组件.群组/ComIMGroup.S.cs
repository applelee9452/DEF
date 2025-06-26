#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemoryPack;

namespace DEF.IM;

// 多实例，不可以加到Scene黑板中
public partial class ComIMGroup : IComponentRpcIMGroup
{
    public Random Rd { get; set; } = new(Guid.NewGuid().GetHashCode());
    ContainerStatefulStream<SStreamInfo> StreamGroup { get; set; }

    void AwakeServer(Dictionary<string, object> create_params)
    {
        Entity.Export4Bson = true;
        Entity.ExportEntityData = true;
        Entity.SetNetworkSyncFlag(true);

        State.MapMember ??= [];
    }

    void OnStartServer()
    {
    }

    void OnDestroyServer(string reason, byte[] user_data)
    {
    }

    void HandleEventServer(DEF.Event ev)
    {
    }

    // 请求修改群名
    Task IComponentRpcIMGroup.ModifyGroupName(string group_name)
    {
        State.GroupName = group_name;

        return Task.CompletedTask;
    }

    // 群初始化
    public void OnSetup(string group_name, GroupMember admin, ContainerStatefulStream<SStreamInfo> stream)
    {
        State.GroupGuid = Scene.ContainerId;
        State.GroupName = group_name;
        State.AdminGuid = admin.PlayerGuid;

        State.MapMember = new()
        {
            { admin.PlayerGuid, admin }
        };

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

        if (State.AdminGuid != player_guid)
        {
            r = IMResult.NoPermission;// 权限不足

            return Task.FromResult(r);
        }

        //State.AdminGuid = string.Empty;
        //State.MapMember = null;
        State.IsDelete = true;
        r = IMResult.Success;

        return Task.FromResult(r);
    }

    // 请求退出该群组
    public Task<IMResult> RequestLeaveGroup(string player_guid, string new_admin_guid)
    {
        IMResult r = IMResult.Error;

        if (State.MapMember != null)
        {
            if (State.MapMember.TryGetValue(player_guid, out var member))
            {
                State.MapMember.Remove(player_guid);

                if (State.AdminGuid == player_guid)
                {
                    if (State.MapMember.Count == 0)
                    {
                        // 只剩管理员了，管理员退出后，该群解散
                        State.IsDelete = true;
                    }
                    else
                    {
                        // 将管理员转给其他成员
                        if (!string.IsNullOrEmpty(new_admin_guid))
                        {
                            if (State.MapMember.TryGetValue(new_admin_guid, out var new_admin))
                            {
                                State.AdminGuid = new_admin_guid;
                            }
                        }
                        else
                        {
                            State.AdminGuid = State.MapMember.First().Value.PlayerGuid;
                        }
                    }
                }

                r = IMResult.Success;
            }
        }

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