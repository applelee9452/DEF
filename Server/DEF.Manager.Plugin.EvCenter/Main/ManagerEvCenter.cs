using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

namespace DEF.Manager;

public class ManagerEvCenter : IManagerPlugin
{
    public ILogger Logger { get; set; }
    public IOptions<DEFOptions> DEFOptions { get; set; }
    public ServiceClient ServiceClient { get; set; }
    public DbClientMongo Db { get; set; }
    ManagerEvCenterConfig ManagerConfig { get; set; }

    public ManagerEvCenter(
        ILogger<ManagerEvCenter> logger,
        IOptions<DEFOptions> def_options,
        ServiceClient service_client)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceClient = service_client;

        string config_path = Utils.GetConfigPath();
        config_path += "DEF.Manager.Plugin.EvCenter.json";
        using TextReader reader = new StreamReader(config_path);
        string s = reader.ReadToEnd();
        ManagerConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ManagerEvCenterConfig>(s);

        Db = new DbClientMongo(ManagerConfig.DBName, ManagerConfig.DbConnectionString);
    }

    public PluginInfo GetPluginInfo()
    {
        PluginInfo info = new()
        {
            Key = "EvCenter",
            Name = "事件中心",
            MinRole = "Admin",
            AssemblyPlugin = typeof(DEF.Manager.Pages.Index).Assembly,
            AssemblyInterface = typeof(DEF.EvCenter.IContainerStatefulInitDb).Assembly,
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
                Text = "事件中心首页",
                Url = "/Plugin/EvCenter/Index",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "客户端错误日志列表",
                Url = "/Plugin/EvCenter/ClientCrashReportList",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        return items;
    }
}