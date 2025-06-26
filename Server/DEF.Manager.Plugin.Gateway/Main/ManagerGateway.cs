using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

namespace DEF.Manager;

public class ManagerGateway : IManagerPlugin
{
    public ILogger Logger { get; set; }
    public IOptions<DEFOptions> DEFOptions { get; set; }
    public ServiceClient ServiceClient { get; set; }
    public DbClientMongo Db { get; set; }
    ManagerGatewayConfig ManagerConfig { get; set; }

    public ManagerGateway(
        ILogger<ManagerGateway> logger,
        IOptions<DEFOptions> def_options,
        ServiceClient service_client)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceClient = service_client;

        string config_path = Utils.GetConfigPath();
        config_path += "DEF.Manager.Plugin.Gateway.json";
        using TextReader reader = new StreamReader(config_path);
        string s = reader.ReadToEnd();
        ManagerConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ManagerGatewayConfig>(s);
    }

    public PluginInfo GetPluginInfo()
    {
        PluginInfo info = new()
        {
            Key = "Gateway",
            Name = "网关",
            MinRole = "Admin",
            AssemblyPlugin = typeof(DEF.Manager.Pages.Index).Assembly,
            AssemblyInterface = typeof(DEF.Gateway.IContainerStatelessGateway).Assembly,
            NavMenuItems = GetNavMenuItems(),
        };

        return info;
    }

    List<NavMenuItem> GetNavMenuItems()
    {
        List<NavMenuItem> items = new();

        {
            NavMenuItem item = new()
            {
                Text = "网关首页",
                Url = "/Plugin/Gateway/Index",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        return items;
    }
}