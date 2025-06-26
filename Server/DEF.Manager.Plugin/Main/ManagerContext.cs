using System.Reflection;
using DEF.Gateway;
using Microsoft.Extensions.Options;

namespace DEF.Manager;

public class ManagerContext
{
    public ILogger Logger { get; set; }
    public IOptions<DEFOptions> DEFOptions { get; set; }
    public IOptions<ManagerOptions> ManagerOptions { get; set; }
    public ServiceClient ServiceClient { get; set; }
    public DbClientMongo DbLogs { get; set; }
    IServiceProvider ServiceProvider { get; set; }
    Dictionary<string, PluginInfo> MapPlugin { get; set; } = [];
    ManagerPlugins ManagerPlugins { get; set; }
    ManagerDb Db { get; set; }

    const int OneDaySecond = 60 * 60 * 24;// 一天的秒数

    public ManagerContext(
        ILogger<ManagerSession> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ManagerOptions> manager_options,
        IServiceProvider service_provider,
        ServiceClient service_client,
        ManagerPlugins manager_plugins,
        ManagerDb db)
    {
        Logger = logger;
        DEFOptions = def_options;
        ManagerOptions = manager_options;
        ServiceProvider = service_provider;
        ServiceClient = service_client;
        ManagerPlugins = manager_plugins;
        Db = db;

        foreach (var t in ManagerPlugins.EntryTypes)
        {
            var manager_plugin = (IManagerPlugin)ServiceProvider.GetService(t);
            var plugin_info = manager_plugin.GetPluginInfo();
            MapPlugin[plugin_info.Key] = plugin_info;
        }

        List<Assembly> list_ass = [];
        list_ass.Add(typeof(IContainerStatelessGateway).Assembly);

        foreach (var i in MapPlugin)
        {
            list_ass.Add(i.Value.AssemblyInterface);
        }
        ServiceClient.Service.Setup(string.Empty, list_ass.ToArray());

        DbLogs = new DbClientMongo(DEFOptions.Value.LogMongoDBName, DEFOptions.Value.LogMongoDBConnectString);
    }

    public PluginInfo GetPluginInfo(string key)
    {
        MapPlugin.TryGetValue(key, out PluginInfo plugin_info);

        return plugin_info;
    }

    public Dictionary<string, PluginInfo> GetPluginMap()
    {
        return MapPlugin;
    }

    // 获得今日起始时间
    DateTime GetTodayStartTime()
    {
        DateTime now = DateTime.UtcNow;
        DateTime start_time = new(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        return start_time;
    }

    // 获得今日EndTime
    DateTime GetTodayEndTime()
    {
        DateTime now = DateTime.UtcNow;
        DateTime start_time = new(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        DateTime end_time = start_time.AddSeconds(OneDaySecond - 1);// 一天的秒数减少1
        return end_time;
    }

    // 获得昨日起始时间
    DateTime GetYestodayStartTime()
    {
        DateTime now = DateTime.UtcNow;
        DateTime start_time = new(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        start_time = start_time.AddDays(-1);// 昨日减少一天
        return start_time;
    }

    // 获得昨日结束时间
    DateTime GetYestodayEndTime()
    {
        DateTime now = DateTime.UtcNow;
        DateTime start_time = new(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        start_time = start_time.AddDays(-1);// 昨日减少一天
        DateTime end_time = start_time.AddSeconds(OneDaySecond - 1);// 一天的秒数减少1
        return end_time;
    }

    // 获得本周周一起始时间
    DateTime GetThisWeekStartTime()
    {
        DateTime now = DateTime.UtcNow;
        DateTime start_time = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        int days = GetWeekDays(start_time);
        start_time = start_time.AddDays(1 - days);  //本周周一
        return start_time;
    }

    // 获得本周周日结束时间
    DateTime GetThisWeekEndTime()
    {
        DateTime now = DateTime.UtcNow;
        DateTime start_time = new(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        int days = GetWeekDays(start_time);
        start_time = start_time.AddDays(1 - days);  //本周周一
        DateTime end_time = start_time.AddDays(6);  //本周周日
        end_time = end_time.AddSeconds(OneDaySecond - 1);// 一天的秒数减少1
        return end_time;
    }

    // 获得上周周一起始时间
    DateTime GetLastWeekStartTime()
    {
        DateTime now = DateTime.UtcNow;
        DateTime start_time = new(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        int days = GetWeekDays(start_time);
        start_time = start_time.AddDays(1 - days - 7);   //上周周一
        return start_time;
    }

    // 获得上周周日结束时间
    DateTime GetLastWeekEndTime()
    {
        DateTime now = DateTime.UtcNow;
        DateTime start_time = new(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        int days = GetWeekDays(start_time);
        start_time = start_time.AddDays(1 - days - 7);   //上周周一
        DateTime end_time = start_time.AddDays(6);  //上周周日
        end_time = end_time.AddSeconds(OneDaySecond - 1);// 一天的秒数减少1
        return end_time;
    }

    // 获得今天是一周的第几天
    int GetWeekDays(DateTime time)
    {
        int days = Convert.ToInt32(time.DayOfWeek.ToString("d"));
        if (days == 0) days = 7;
        return days;
    }
}