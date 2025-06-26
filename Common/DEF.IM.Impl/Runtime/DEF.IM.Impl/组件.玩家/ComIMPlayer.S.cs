#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace DEF.IM;

public partial class ComIMPlayer : IComponentRpcIMPlayer
{
    public Dictionary<string, ComIMPlayerGroup> MapIMPlayerGroup { get; set; }
    public Random Rd { get; set; } = new(Guid.NewGuid().GetHashCode());
    public string GatewayGuid { get; private set; } = string.Empty;
    public string SessionGuid { get; private set; } = string.Empty;
    public bool ClientOnline { get; private set; } = false;// Client是否在线
    ContainerStatefulIMPlayer ContainerStatefulIMPlayer { get; set; }
    Stopwatch StopwatchOnline { get; set; }// 玩家在线时长秒数计算

    //float WriteDbTm { get; set; } = 0;

    void AwakeServer(Dictionary<string, object> create_params)
    {
        Entity.Export4Bson = true;
        Entity.ExportEntityData = true;
        Entity.SetNetworkSyncFlag(true);

        if (State.LastLoginDt == DateTime.MinValue)
        {
            State.LastLoginDt = DateTime.UtcNow;
        }

        if (State.JoinDt == DateTime.MinValue)
        {
            State.JoinDt = DateTime.UtcNow;
        }

        StopwatchOnline = new Stopwatch();
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

    // GM命令
    async Task<string> IComponentRpcIMPlayer.GMCommand(string command, string param1, string param2, string param3)
    {
        string result = string.Empty;

        await Task.Delay(1);

        if (command == "addexp")
        {
            if (!int.TryParse(param1, out var v))
            {
                result = "addexp失败，参数错误，参数1是经验值是整数";
                return result;
            }

            var exp = State.Exp;
            exp += v;
            if (exp < 0) exp = 0;
            State.Exp = exp;

            result = "addexp成功，增加" + v + "经验";
        }
        else if (command == "addgold")
        {
            if (!int.TryParse(param1, out var v))
            {
                result = "addgold失败，参数错误，参数1是金币数量是整数";
                return result;
            }

            var gold = State.Gold;
            gold += v;
            if (gold < 0) gold = 0;
            State.Gold = gold;

            result = "addgold成功，增加" + v + "金币";
        }
        else if (command == "adddiamond")
        {
            if (!int.TryParse(param1, out var v))
            {
                result = "adddiamond失败，参数错误，参数1是钻石数量是整数";
                return result;
            }

            var diamond = State.Diamond;
            diamond += v;
            if (diamond < 0) diamond = 0;
            State.Diamond = diamond;

            result = "adddiamond成功，增加" + v + "钻石";
        }

        return result;
    }

    // 请求修改昵称
    Task IComponentRpcIMPlayer.RequestChangeNickname(string nickname)
    {
        State.NickName = nickname;

        return Task.CompletedTask;
    }

    // 请求创建群组
    async Task<CreateGroupResult> IComponentRpcIMPlayer.RequestCreateGroup(string group_name)
    {
        CreateGroupResult r = new()
        {
            Result = IMResult.Error,
            GroupGuid = string.Empty,
            GroupName = string.Empty,
        };

        if (string.IsNullOrEmpty(group_name))
        {
            r.Result = IMResult.ParamError;

            return r;
        }

        if (group_name.Length > 128)
        {
            r.Result = IMResult.IllegalRequest;

            return r;
        }

        // todo，创建群组规则判定
        // 例如：我的等级>=30级，且我现在不是任何群的群主

        string group_guid = Guid.NewGuid().ToString();

        GroupMember admin = new()
        {
            PlayerGuid = State.PlayerGuid,
            NickName = State.NickName,
            Icon = State.Icon,
        };

        var c = GetContainerRpc<IContainerStatefulIMGroup>(group_guid);
        r = await c.Setup(group_name, admin);

        if (r.Result == IMResult.Success)
        {
            Dictionary<string, object> create_params = new()
            {
                { "R", r }
            };
            var et_implayergroup = Scene.CreateEntity<ComIMPlayerGroup>("IMPlayerGroup", Entity, create_params);
            var com_implayergroup = et_implayergroup.GetComponent<ComIMPlayerGroup>();

            await com_implayergroup.SubGroup(ContainerStatefulIMPlayer);
        }

        return r;
    }

    // 请求加入群组
    Task IComponentRpcIMPlayer.RequestJoinGroup(string group_guid)
    {
        // todo，加入群组规则

        return Task.CompletedTask;
    }

    // IM玩家加载
    public async Task OnCreate(ContainerStatefulIMPlayer c)
    {
        ContainerStatefulIMPlayer = c;

        var com_implayerregion = Scene.GetComponentFromBlackboard<ComIMPlayerRegion>();
        await com_implayerregion.SubRegion(c);

        var com_im_player_mailbox = Scene.GetComponentFromBlackboard<ComIMPlayerMailBox>();
        await com_im_player_mailbox.GetLastestSystemMailList();
    }

    // IM玩家新建
    public async Task OnNewPlayer(ContainerStatefulIMPlayer container_implayer)
    {
        State.PlayerGuid = Scene.ContainerId;

        if (IsBot())
        {
            State.NickName = Scene.ContainerId;
        }

        // UID生成
        var filter = Builders<ConfigUniqId>.Filter
            .Where(x => x._id.Equals(StringDef.DocConfigUniqId));
        var update = Builders<ConfigUniqId>.Update
            .Inc<ulong>(x => x.UniqPlayerId, 1);
        var collection = IMContext.Instance.Mongo.Database.GetCollection<ConfigUniqId>(StringDef.DbCollectionConfigUniqId);
        FindOneAndUpdateOptions<ConfigUniqId> fuo = new() { IsUpsert = true };
        var doc = await collection.FindOneAndUpdateAsync(filter, update, fuo);
        State.UId = doc.UniqPlayerId;

        // 自动分配分区
        var c = GetContainerRpc<IContainerStatefulIMRegionMgr>();
        var region = await c.RequestAssignReion();

        // 赋予该玩家分区
        var com_implayerregion = Scene.GetComponentFromBlackboard<ComIMPlayerRegion>();
        await com_implayerregion.AssignRegion(container_implayer, region);
    }

    // 客户端上线
    public async Task ServerOnClientAttached(DEF.Gateway.GatewayAuthedInfo info, string extra_data)
    {
        Logger.LogInformation("ComIMPlayer.OnClientAttached() PlayerGuid={PlayerGuid} GatewayGuid={GatewayGuid} SessionGuid={SessionGuid}", State.PlayerGuid, info.GatewayGuid, info.SessionGuid);

        GatewayGuid = info.GatewayGuid;
        SessionGuid = info.SessionGuid;
        ClientOnline = true;

        Scene.ClearNetworkSyncInfo();
        Scene.ClearAllSubClient();
        Scene.AddSubClient(SessionGuid, GatewayGuid);
        Scene.NetworkSyncFlag = true;

        State.ClientIp = info.ClientIp;
        State.IpAddress = info.ClientIpAddress;
        State.LastLoginDt = DateTime.UtcNow;

        StopwatchOnline.Restart();

        bool is_bot = IsBot();
        if (!is_bot)
        {
            if (string.IsNullOrEmpty(State.NickName))
            {
                State.NickName = info.NickName;
            }
        }

        Scene.ClearNetworkSyncInfo();

        if (!string.IsNullOrEmpty(GatewayGuid) && !string.IsNullOrEmpty(SessionGuid))
        {
            var entity_data = Entity.GetEntityData(SessionGuid);

            // 推送玩家快照
            var ob = GetContainerRpcObserver<IContainerObserverDEF>(GatewayGuid, SessionGuid);
            await ob.SyncSceneSnapshot2Client(Scene.Name, (EntityData)entity_data);
        }
    }

    // 客户端下线
    public async Task ServerOnClientDeattached(string session_guid, ContainerStatefulIMPlayer c)
    {
        Logger.LogInformation("ComIMPlayer.OnClientDeattached() PlayerGuid={PlayerGuid} GatewayGuid={GatewayGuid} SessionGuid={SessionGuid}", State.PlayerGuid, GatewayGuid, SessionGuid);

        if (SessionGuid != session_guid)
        {
            return;
        }

        ulong online_seconds = (ulong)StopwatchOnline.Elapsed.TotalSeconds;
        StopwatchOnline.Restart();

        Scene.NetworkSyncFlag = false;
        Scene.ClearNetworkSyncInfo();
        Scene.ClearAllSubClient();

        GatewayGuid = string.Empty;
        SessionGuid = string.Empty;
        ClientOnline = false;
        bool is_bot = IsBot();

        // 保存到Db
        await Entity.SyncDelta2Db(IMContext.Instance.Mongo.Database, StringDef.DbCollectionEntityPlayer);

        c.DeactivateOnIdle1();
    }

    // 收到跑马灯消息
    public Task RecvMarquee(BIMMarquee marquee)
    {
        var ob = GetEntityRpcObserver<IComponentObserverIMPlayer>(GatewayGuid, SessionGuid);
        return ob.RecvMarquee(marquee);
    }

    // 请求获取玩家信息
    public PlayerInfo GetPlayerInfo()
    {
        PlayerInfo info = new()
        {
            PlayerGuid = State.PlayerGuid,
            NickName = State.NickName,
            Icon = State.Icon,
            IpAddress = State.IpAddress,
            UId = State.UId
        };

        return info;
    }

    // 获取分区信息
    public Region GetRegionInfo()
    {
        var com_implayerregion = Scene.GetComponentFromBlackboard<ComIMPlayerRegion>();

        Region region = new()
        {
            RegionGuid = com_implayerregion.State.RegionGuid,
            RegionId = com_implayerregion.State.RegionId,
            Dt = com_implayerregion.State.RegionDt,
        };

        return region;
    }

    // 每帧更新
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

    public bool IsBot()
    {
        if (int.TryParse(Scene.ContainerId, out _))
        {
            return true;
        }

        return false;
    }
}

#endif