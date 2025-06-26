using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DEF.Manager;

public class ManagerService : IHostedService
{
    public ILogger Logger { get; set; }
    public IOptions<DEFOptions> DEFOptions { get; set; }
    public IOptions<ManagerOptions> ManagerOptions { get; set; }
    public DbClientMongo DbLogs { get; set; }
    IServiceProvider ServiceProvider { get; set; }
    Dictionary<string, PluginInfo> MapPlugin { get; set; } = [];
    ManagerPlugins ManagerPlugins { get; set; }
    ManagerDb Db { get; set; }

    public ManagerService(
        ILogger<ManagerSession> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ManagerOptions> manager_options,
        ManagerDb db)
    {
        Logger = logger;
        DEFOptions = def_options;
        ManagerOptions = manager_options;
        Db = db;
    }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        // 初始化数据库

        var init_db = ManagerOptions.Value.InitDb;
        if (init_db != null)
        {
            if (init_db.ListRole != null && init_db.ListRole.Count > 0)
            {
                await Db.CreateRoles(init_db.ListRole);
            }

            if (init_db.ListUser != null && init_db.ListUser.Count > 0)
            {
                await Db.CreateAdmin(init_db.ListUser);
            }
        }
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}