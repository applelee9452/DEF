using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEF;

public class OrleansStartup : IStartupTask
{
    ILogger Logger { get; set; }
    IGrainFactory GrainFactory { get; set; }
    IServiceProvider ServiceProvider { get; set; }
    IService Service { get; set; }
    ServiceNode ServiceNode { get; set; }

    //readonly ServiceNodeAssembly ServiceNodeAssembly;

    public OrleansStartup(IGrainFactory grain_factory,
        ILogger<OrleansStartup> logger,
        IServiceProvider service_provider,
        Service service,
        ServiceNode service_node)
    {
        GrainFactory = grain_factory;
        Logger = logger;
        ServiceProvider = service_provider;
        Service = service;
        ServiceNode = service_node;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        //if (Test.Config.ConfigLobby.IsTest)
        //{
        //    Logger.LogInformation("当前运行模式为：{0}模式！", "测试");
        //}

        // 启动GrainTestBootstrap
        //if (Test.Config.ConfigLobby.IsTest && Test.Config.ConfigLobby.TestBootstrap)
        //{
        //    var grain_test_bootstrap = GrainFactory.GetGrain<IGrainTestBootstrap>(StringDef.GrainTestBootstrap);
        //    await grain_test_bootstrap.Test1();
        //}

        // 本地调用
        //var grain_config_sqlite_service = GrainFactory.GetGrain<IGrainDownloadSqliteDbService>(0);
        //await grain_config_sqlite_service.DownloadThenParseSqliteDbFromOss();

        // 集群调用
        //var grain_launch = GrainFactory.GetGrain<IGrainLaunch>(StringDef.GrainLaunch);
        //await grain_launch.Launch();

        // 启动完成
        //await LobbyContext.Instance.OnStarted();

        //await ServiceAssembly.Run(GrainFactory);

        if (ServiceNode.FuncOrleansStartupExcute != null)
        {
            await ServiceNode.FuncOrleansStartupExcute();
        }
    }
}