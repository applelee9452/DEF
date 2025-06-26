using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DEF.UCenter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DEF.Manager;

public class ManagerUCenter : IManagerPlugin
{
    public ILogger Logger { get; private set; }
    public IOptions<DEFOptions> DEFOptions { get; private set; }
    public ServiceClient ServiceClient { get; private set; }
    public DbClientMongo Db { get; private set; }
    public DbClientMongo DbGame { get; private set; }
    public ManagerUCenterConfig ManagerConfig { get; private set; }
    Dictionary<string, UCenterPlayerInfo> UCenterPlayerInfoDict { get; set; } = new();
    Dictionary<ulong, DataAgent> DataAgentDict { get; set; } = new();

    public ManagerUCenter(
        ILogger<ManagerUCenter> logger,
        IOptions<DEFOptions> def_options,
        ServiceClient service_client)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceClient = service_client;

        string config_path = Utils.GetConfigPath();
        config_path += "DEF.Manager.Plugin.UCenter.json";
        using TextReader reader = new StreamReader(config_path);
        string s = reader.ReadToEnd();
        ManagerConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ManagerUCenterConfig>(s);

        Db = new DbClientMongo(ManagerConfig.DBName, ManagerConfig.DbConnectionString);
        DbGame = new DbClientMongo(ManagerConfig.MaoDBName, ManagerConfig.DbConnectionString);
    }

    public PluginInfo GetPluginInfo()
    {
        PluginInfo info = new()
        {
            Key = "UCenter",
            Name = "用户中心",
            MinRole = "Agent",
            AssemblyPlugin = typeof(DEF.Manager.Pages.Index).Assembly,
            AssemblyInterface = typeof(DEF.UCenter.IContainerStatelessAccount).Assembly,
            NavMenuItems = GetNavMenuItems(),
        };
        return info;
    }

    List<NavMenuItem> GetNavMenuItems()
    {
        List<NavMenuItem> items = [];

        {
            NavMenuItem item = new()
            {
                Text = "用户中心首页",
                Url = "/Plugin/UCenter/Index",
                Icon = "oi oi-home",
                MinRole = "Agent"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "代理链接",
                Url = "/Plugin/UCenter/AgentLink",
                Icon = "oi oi-home",
                MinRole = "Agent"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "代理列表",
                Url = "/Plugin/UCenter/AgentList",
                Icon = "oi oi-home",
                MinRole = "Agent"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "用户列表",
                Url = "/Plugin/UCenter/AccountList",
                Icon = "oi oi-home",
                MinRole = "Agent"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "用户新增报表",
                Url = "/Plugin/UCenter/AccountTableCreate",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "用户活跃报表",
                Url = "/Plugin/UCenter/AccountTableActive",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "用户留存报表",
                Url = "/Plugin/UCenter/AccountTableRetention",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "用户流失报表",
                Url = "/Plugin/UCenter/AccountTableChurn",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "支付订单列表",
                Url = "/Plugin/UCenter/PayChargeList",
                Icon = "oi oi-home",
                MinRole = "Agent"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "配置",
                Url = "/Plugin/UCenter/Settings",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        //{
        //    NavMenuItem item = new()
        //    {
        //        Text = "测试",
        //        Url = "/Plugin/UCenter/Test",
        //        Icon = "oi oi-home",
        //        MinRole = "Admin"
        //    };
        //    items.Add(item);
        //}

        return items;
    }

    public UCenterPlayerInfo GetPlayerInfoFromBsonDocument(BsonDocument elements)
    {
        UCenterPlayerInfo player_info = new();

        try
        {
            player_info.PlayerGuid = elements["States"][0]["State"]["PlayerGuid"].ToString();
            player_info.PlayerUid = elements["States"][0]["State"]["UId"].ToString();
            player_info.PlayerNickName = elements["States"][0]["State"]["NickName"].ToString();
        }
        catch (Exception)
        {
        }

        return player_info;
    }

    public async Task<UCenterPlayerInfo> GetPlayerInfoByGuid(string player_guid)
    {
        if (string.IsNullOrEmpty(player_guid))
            return new UCenterPlayerInfo() { PlayerNickName = "Null", PlayerUid = "Null" };

        if (UCenterPlayerInfoDict.TryGetValue(player_guid, out UCenterPlayerInfo info))
        {
            return info;
        }

        var collection = DbGame.Database.GetCollection<BsonDocument>("EntityPlayer");
        FilterDefinition<BsonDocument> filter_p = Builders<BsonDocument>.Filter.ElemMatch("States", Builders<BsonValue>.Filter.Eq("State.PlayerGuid", player_guid));

        try
        {
            var entity_player_document_list = await collection.Find<BsonDocument>(filter_p, null).ToListAsync();
            if (entity_player_document_list.Count == 0)
            {
                return null;
            }

            var elements = entity_player_document_list[0];
            string uid = elements["States"][0]["State"]["UId"].ToString();
            string nickname = elements["States"][0]["State"]["NickName"].ToString();

            UCenterPlayerInfo ucenter_player = new UCenterPlayerInfo();
            ucenter_player.PlayerGuid = player_guid;
            ucenter_player.PlayerNickName = nickname;
            ucenter_player.PlayerUid = uid;
            UCenterPlayerInfoDict.Add(ucenter_player.PlayerGuid, ucenter_player);

            return ucenter_player;
        }
        catch (Exception)
        {
            UCenterPlayerInfo ucenter_player = new UCenterPlayerInfo();
            ucenter_player.PlayerGuid = player_guid;
            UCenterPlayerInfoDict.Add(ucenter_player.PlayerGuid, ucenter_player);

            return ucenter_player;
        }
    }

    public async Task<UCenterPlayerInfo> GetPlayerInfoByUId(long player_uid)
    {
        UCenterPlayerInfo player_info = new();
        var collection = DbGame.Database.GetCollection<BsonDocument>("EntityPlayer");
        FilterDefinition<BsonDocument> filter_p = Builders<BsonDocument>.Filter.ElemMatch("States", Builders<BsonValue>.Filter.Eq("State.UId", player_uid));

        try
        {
            var entity_player_document_list = await collection.Find<BsonDocument>(filter_p, null).ToListAsync();
            if (entity_player_document_list.Count == 0)
            {
                return null;
            }

            var elements = entity_player_document_list[0];
            player_info = GetPlayerInfoFromBsonDocument(elements);

            return player_info;
        }
        catch (Exception)
        {
            return player_info;
        }
    }

    public async Task<UCenterPlayerInfo> GetPlayerInfoByNickName(string nickname)
    {
        UCenterPlayerInfo player_info = new UCenterPlayerInfo();
        var collection = DbGame.Database.GetCollection<BsonDocument>("EntityPlayer");
        FilterDefinition<BsonDocument> filter_p = Builders<BsonDocument>.Filter.ElemMatch("States", Builders<BsonValue>.Filter.Regex("State.NickName", new BsonRegularExpression(nickname, "i")));

        try
        {
            var entity_player_document_list = await collection.Find<BsonDocument>(filter_p, null).ToListAsync();
            if (entity_player_document_list.Count == 0)
            {
                return null;
            }

            var elements = entity_player_document_list[0];
            player_info = GetPlayerInfoFromBsonDocument(elements);

            return player_info;
        }
        catch (Exception)
        {
            return player_info;
        }
    }

    public async Task<DataAgentView> GetDataAgentViewByName(string username)
    {
        var collection = Db.Database.GetCollection<DataAgent>(StringDef.DbCollectionDataAgent);

        var filter = Builders<DataAgent>.Filter.Where(i => i.UserName == username);

        var list = await collection.Find(filter, null).ToListAsync();

        if (list.Count == 0)
        {
            return null;
        }

        DataAgent data_agent = list.First();

        DataAgentView data_agent_view = new()
        {
            AgentId = data_agent.AgentId,
            UserName = data_agent.UserName,
            AgentParents = data_agent.AgentParents
        };

        for (int i = 0; i < data_agent.AgentParents?.Length; i++)
        {
            var agent_parent_id = data_agent.AgentParents[i];

            var p_data_agent = await GetDataAgentById(agent_parent_id);

            data_agent_view.StrAgentParent += $"{p_data_agent.UserName}->";
        }

        data_agent_view.StrAgentParent += $"{data_agent.UserName}";

        return data_agent_view;
    }

    public async Task<DataAgent> GetDataAgentById(ulong agent_id)
    {
        if (DataAgentDict.TryGetValue(agent_id, out DataAgent data))
        {
            return data;
        }

        DataAgent data_agent = null;

        var collection = Db.Database.GetCollection<DataAgent>(StringDef.DbCollectionDataAgent);

        FilterDefinition<DataAgent> filter = Builders<DataAgent>.Filter.Where(i => i.AgentId == agent_id);

        var list = await collection.Find<DataAgent>(filter, null).ToListAsync();

        if (list.Count == 0)
        {
            data_agent = new DataAgent() { Id = "agent_id", UserName = "未知" };
        }
        else
        {
            data_agent = list.First();
        }

        if (DataAgentDict.ContainsKey(agent_id))
        {
            return data_agent;
        }

        DataAgentDict.Add(agent_id, data_agent);

        return data_agent;
    }
}