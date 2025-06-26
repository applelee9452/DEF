using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace DEF.EvCenter;

public class EvCenterContext : IHostedService
{
    public static EvCenterContext Instance { get; private set; }

    public DbClientMongo Mongo;
    public readonly ILogger Logger;
    readonly IServiceProvider ServiceProvider;
    readonly IService Service;
    readonly ServiceNode ServiceNode;
    readonly IOptions<DEFOptions> DEFOptions;
    readonly IOptions<ServiceOptions> ServiceOptions;

    public EvCenterContext(ILogger<EvCenterContext> logger,
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

        var object_serializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("EvCenter"));
        BsonSerializer.RegisterSerializer(object_serializer);

        var def_service = ServiceProvider.GetRequiredService<Service>();
        var assembly1 = typeof(IContainerStatelessEvCenter).Assembly;
        var assembly2 = typeof(IContainerStatelessEvClientException).Assembly;
        var assembly3 = typeof(ContainerStatelessEvClientException).Assembly;
        def_service.Setup(ServiceOptions.Value.ServiceName, assembly1, assembly2, assembly3);

        ServiceNode.FuncOrleansStartupExcute += async () =>
        {
            Logger.LogDebug("OrleansStartup Begin");

            // 初始化所有第三方组件
            Mongo = new DbClientMongo(
                ServiceOptions.Value.MongoDBName,
                ServiceOptions.Value.MongoDBConnectString);

            var c = Service.GetContainerRpc<IContainerStatefulInitCluster>();
            await c.Touch();

            Logger.LogDebug("OrleansStartup End");
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("EvCenterContext启动成功！");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("EvCenterContext停止成功！");

        return Task.CompletedTask;
    }
}