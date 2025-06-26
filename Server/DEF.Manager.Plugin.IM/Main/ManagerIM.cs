using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

namespace DEF.Manager;

public class ManagerIM : IManagerPlugin
{
    public ILogger Logger { get; set; }
    public IOptions<DEFOptions> DEFOptions { get; set; }
    public ServiceClient ServiceClient { get; set; }
    public DbClientMongo Db { get; set; }
    ManagerIMConfig ManagerConfig { get; set; }

    public ManagerIM(
        ILogger<ManagerIM> logger,
        IOptions<DEFOptions> def_options,
        ServiceClient service_client)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceClient = service_client;

        string config_path = Utils.GetConfigPath();
        config_path += "DEF.Manager.Plugin.IM.json";
        using TextReader reader = new StreamReader(config_path);
        string s = reader.ReadToEnd();
        ManagerConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ManagerIMConfig>(s);

        Db = new DbClientMongo(ManagerConfig.DBName, ManagerConfig.DbConnectionString);
    }

    public PluginInfo GetPluginInfo()
    {
        PluginInfo info = new()
        {
            Key = "IM",
            Name = "社交",
            MinRole = "Admin",
            AssemblyPlugin = typeof(DEF.Manager.Pages.Index).Assembly,
            AssemblyInterface = typeof(DEF.IM.IContainerStatefulIMPlayer).Assembly,
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
                Text = "社交首页",
                Url = "/Plugin/IM/Index",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "跑马灯",
                Url = "/Plugin/IM/Marquee",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "系统公告",
                Url = "/Plugin/IM/SystemNotice",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "系统邮箱",
                Url = "/Plugin/IM/SystemMailBox",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "CDKey管理",
                Url = "/Plugin/IM/CDKeyMgr",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "CDKey领取列表",
                Url = "/Plugin/IM/CDKeyExchangeList",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "分区管理",
                Url = "/Plugin/IM/RegionMgr",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        {
            NavMenuItem item = new()
            {
                Text = "控制台",
                Url = "/Plugin/IM/Console",
                Icon = "oi oi-home",
                MinRole = "Admin"
            };
            items.Add(item);
        }

        return items;
    }
}