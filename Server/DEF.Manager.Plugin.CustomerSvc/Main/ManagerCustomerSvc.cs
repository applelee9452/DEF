using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

namespace DEF.Manager;

public class ManagerCustomerSvc : IManagerPlugin
{
    public ILogger Logger { get; set; }
    public IOptions<DEFOptions> DEFOptions { get; set; }
    public ServiceClient ServiceClient { get; set; }
    public DbClientMongo Db { get; set; }
    ManagerCustomerSvcConfig ManagerConfig { get; set; }

    public ManagerCustomerSvc(
        ILogger<ManagerCustomerSvc> logger,
        IOptions<DEFOptions> def_options,
        ServiceClient service_client)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceClient = service_client;

        string config_path = Utils.GetConfigPath();
        config_path += "DEF.Manager.Plugin.CustomerSvc.json";
        using TextReader reader = new StreamReader(config_path);
        string s = reader.ReadToEnd();
        ManagerConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ManagerCustomerSvcConfig>(s);

        Db = new DbClientMongo(ManagerConfig.DBName, ManagerConfig.DbConnectionString);
    }

    public PluginInfo GetPluginInfo()
    {
        PluginInfo info = new()
        {
            Key = "CustomerSvc",
            Name = "客服",
            MinRole = "Admin",
            //AssemblyPlugin = typeof(DEF.Manager.Pages.Index).Assembly,
            //AssemblyInterface = typeof(DEF.CCenter.IContainerManager).Assembly,
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
                Text = "客服首页",
                Url = "/Plugin/CustomerSvc/Index",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        return items;
    }
}