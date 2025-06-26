#if !DEF_CLIENT

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DEF.IM;

public class IMContext : IHostedService
{
    public static IMContext Instance { get; private set; }
    public DbClientMongo Mongo { get; private set; }
    public DbClientRedis DbClientRedis { get; private set; }
    public ILogger Logger { get; private set; }
    IService Service { get; set; }
    ServiceNode ServiceNode { get; set; }
    IServiceProvider ServiceProvider { get; set; }
    public IOptions<DEFOptions> DEFOptions { get; set; }
    IOptions<ServiceOptions> ServiceOptions { get; set; }

    public IMContext(ILogger<IMContext> logger,
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

        var object_serializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("DEF.IM"));
        BsonSerializer.RegisterSerializer(object_serializer);

        var def_service = ServiceProvider.GetRequiredService<Service>();
        var assembly1 = typeof(IContainerObserverDEF).Assembly;
        var assembly2 = typeof(IContainerStatefulIMPlayer).Assembly;
        var assembly3 = typeof(ContainerStatefulIMPlayer).Assembly;
        def_service.Setup(ServiceOptions.Value.ServiceName, assembly1, assembly2, assembly3);

        ServiceNode.FuncOrleansStartupExcute += async () =>
        {
            Logger.LogDebug("OrleansStartup Begin");

            // 初始化所有第三方组件
            Mongo = new DbClientMongo(
                ServiceOptions.Value.MongoDBName,
                ServiceOptions.Value.MongoDBConnectString);

            DbClientRedis = new(IMContext.Instance.DEFOptions.Value.RedisName, DEFOptions.Value.RedisConnectString, DEFOptions.Value.Timezone);

            Logger.LogDebug("OrleansStartup End");

            var c = Service.GetContainerRpc<IContainerStatefulIMCluster>();
            await c.Setup();

            //var c = Service.GetContainerRpc<IContainerStatefulTestMgr>();
            //await c.Touch();
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("IMContext启动成功！");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("IMContext停止成功！");

        return Task.CompletedTask;
    }
}

#endif