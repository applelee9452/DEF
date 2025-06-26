using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

namespace DEF.Manager;

public class ManagerCCenter : IManagerPlugin
{
    public ILogger Logger { get; set; }
    public IOptions<DEFOptions> DEFOptions { get; set; }
    public ServiceClient ServiceClient { get; set; }
    public DbClientMongo Db { get; set; }
    ManagerCCenterConfig ManagerConfig { get; set; }

    public ManagerCCenter(
        ILogger<ManagerCCenter> logger,
        IOptions<DEFOptions> def_options,
        ServiceClient service_client)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceClient = service_client;

        string config_path = Utils.GetConfigPath();
        config_path += "DEF.Manager.Plugin.CCenter.json";
        using TextReader reader = new StreamReader(config_path);
        string s = reader.ReadToEnd();
        ManagerConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ManagerCCenterConfig>(s);

        Db = new DbClientMongo(ManagerConfig.DBName, ManagerConfig.DbConnectionString);
    }

    public PluginInfo GetPluginInfo()
    {
        PluginInfo info = new()
        {
            Key = "CCenter",
            Name = "配置中心",
            MinRole = "Admin",
            AssemblyPlugin = typeof(DEF.Manager.Pages.Index).Assembly,
            AssemblyInterface = typeof(DEF.CCenter.IContainerManager).Assembly,
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
                Text = "配置中心首页",
                Url = "/Plugin/CCenter/Index",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "名字空间列表",
                Url = "/Plugin/CCenter/NameSpaceList",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "配置列表",
                Url = "/Plugin/CCenter/CfgList",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        return items;
    }
}