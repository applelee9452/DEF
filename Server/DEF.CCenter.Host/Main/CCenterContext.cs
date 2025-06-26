using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DEF.CCenter;

public class CCenterContext : IHostedService
{
    public static CCenterContext Instance { get; private set; }
    public DbClientMongo Mongo { get; private set; }
    public ILogger Logger { get; private set; }
    IService Service { get; set; }
    ServiceNode ServiceNode { get; set; }
    IServiceProvider ServiceProvider { get; set; }
    IOptions<DEFOptions> DEFOptions { get; set; }
    IOptions<ServiceOptions> ServiceOptions { get; set; }

    public CCenterContext(ILogger<CCenterContext> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ServiceOptions> service_options,
        IServiceProvider service_provider,
        Service service,
        ServiceNode service_node)
    {
        Instance = this;
        Logger = logger;
        DEFOptions = def_options;
        ServiceOptions = service_options;
        ServiceProvider = service_provider;
        Service = service;
        ServiceNode = service_node;

        var def_service = ServiceProvider.GetRequiredService<Service>();
        var assembly1 = typeof(IContainerStatelessCCenter).Assembly;
        var assembly2 = typeof(IContainerManager).Assembly;
        var assembly3 = typeof(ContainerStatelessManager).Assembly;
        def_service.Setup(ServiceOptions.Value.ServiceName, assembly1, assembly2, assembly3);

        ServiceNode.FuncOrleansStartupExcute += async () =>
        {
            Logger.LogDebug("OrleansStartup Begin");

            // 初始化所有第三方组件
            Mongo = new DbClientMongo(
                ServiceOptions.Value.MongoDBName,
                ServiceOptions.Value.MongoDBConnectString);

            var container_initdb = Service.GetContainerRpc<IContainerInitDb>();
            await container_initdb.Touch();

            Logger.LogDebug("OrleansStartup End");
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("CCenterContext启动成功！");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("CCenterContext停止成功！");

        return Task.CompletedTask;
    }
}
