using DEF.Cloud;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DEF.UCenter;

public class UCenterContext : IHostedService
{
    public static UCenterContext Instance { get; private set; }
    public IStorage StorageContext { get; private set; }
    public DbClientMongo Db { get; set; }
    public DbClientRedis DbClientRedis { get; private set; }
    public ILogger Logger { get; set; }
    public IHttpClientFactory HttpClientFactory { get; set; }
    public IOptions<DEFOptions> DEFOptions { get; set; }
    public IOptions<ServiceOptions> ServiceOptions { get; set; }
    public IOptions<UCenterOptions> UCenterOptions { get; set; }
    public IdGenerator.IIdGenerator IdGen { get; private set; }
    IServiceProvider ServiceProvider { get; set; }
    IService Service { get; set; }
    ServiceNode ServiceNode { get; set; }
    Dictionary<string, ConfigApp> MapAppId { get; set; } = [];// Key=AppId

    public UCenterContext(ILogger<UCenterContext> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ServiceOptions> service_options,
        IOptions<UCenterOptions> ucenter_options,
        IServiceProvider service_provider,
        IHttpClientFactory httpclient_factory,
        Service service,
        ServiceNode service_node)
    {
        Instance = this;
        Logger = logger;
        DEFOptions = def_options;
        ServiceOptions = service_options;
        UCenterOptions = ucenter_options;
        ServiceProvider = service_provider;
        HttpClientFactory = httpclient_factory;
        Service = service;
        ServiceNode = service_node;

        var idgenerator_options = new IdGenerator.IdGeneratorOptions()
        {
            Method = 1,
            WorkerId = 1,
            WorkerIdBitLength = 6,
            SeqBitLength = 12,
            //TopOverCostCount = 2000,
            //DataCenterIdBitLength = 0,
            //TimestampType = 1,
            // MinSeqNumber = 1,
            // MaxSeqNumber = 200,
            // BaseTime = DateTime.UtcNow.AddYears(-10),
        };

        IdGen = new IdGenerator.DefaultIdGenerator(idgenerator_options);

        var def_service = ServiceProvider.GetRequiredService<Service>();

        var assembly1 = typeof(IContainerStatelessUCenter).Assembly;
        var assembly2 = typeof(IContainerStatelessAccount).Assembly;
        var assembly3 = typeof(IContainerStatelessApp).Assembly;
        var assembly4 = typeof(ContainerStatelessApp).Assembly;
        def_service.Setup(ServiceOptions.Value.ServiceName, assembly1, assembly2, assembly3, assembly4);

        ServiceNode.FuncOrleansStartupExcute += async () =>
        {
            Logger.LogInformation("OrleansStartup Begin");

            // 初始化MongoClient
            Db = new DbClientMongo(
                ServiceOptions.Value.MongoDBName,
                ServiceOptions.Value.MongoDBConnectString);

            // 初始化RedisClient
            DbClientRedis = new(UCenterContext.Instance.DEFOptions.Value.RedisName, DEFOptions.Value.RedisConnectString, DEFOptions.Value.Timezone);

            // 阿里云短信
            DEF.Cloud.Service.ConfigSms(CloudType.Aliyun);

            //// Storage
            //if (UCenterOptions.Value.StorageType == "Storage.Ali")
            //{
            //    StorageContext = new StorageAliyun();
            //}
            //else
            //{
            //    StorageContext = new StorageAzure();
            //}

            var container_cluster = Service.GetContainerRpc<IContainerStatefulCluster>();
            await container_cluster.Setup();

            var apps = await Db.ReadListAsync<ConfigApp>(e => !string.IsNullOrEmpty(e.Id), StringDef.DbCollectionConfigApp);

            if (apps.Count == 0)
            {
                var app = new ConfigApp
                {
                    Id = "Test",
                    Name = "Test",
                    WechatAppId = "",
                    WechatAppSecret = "",
                    WechatMpAppId = "",
                    WechatMpAppSecret = "",
                    TaptapAppId = "",
                    AliOssBucketName = "a",
                    FacebookAppId = "",
                    FacebookSecret = "",
                    FacebookAccessToken = "",
                    GoogleAppId = "com.XX.XXX",
                };

                await Db.InsertAsync(StringDef.DbCollectionConfigApp, app);

                apps.Add(app);
            }

            foreach (var i in apps)
            {
                if (!string.IsNullOrEmpty(i.Id)) MapAppId[i.Id] = i;

                Console.WriteLine($"AppId={i.Id}\nWechatAppId={i.WechatAppId}\nWechatMpAppId={i.WechatMpAppId}\nTaptapAppId={i.TaptapAppId}\nAliOssBucketName={i.AliOssBucketName}");

                //await StorageContext.CreateBucketIfNotExists(i.AliOssBucketName);
            }

            //bool enable_test = UCenterOptions.Value.EnableTest;
            //if (enable_test)
            //{
            //    var container_test = Service.GetContainerRpc<IContainerStatefulTest>();
            //    await container_test.Setup();
            //}

            Logger.LogInformation("OrleansStartup End");
        };
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("UCenterContext启动成功！");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("UCenterContext停止成功！");

        return Task.CompletedTask;
    }

    public ConfigApp GetAppEntityByAppId(string app_id)
    {
        MapAppId.TryGetValue(app_id, out var app_entity);
        return app_entity;
    }

    public ConfigApp GetAppEntityByWechatAppId(string wechat_appid)
    {
        MapAppId.TryGetValue(wechat_appid, out var app_entity);
        return app_entity;
    }

    public ConfigApp GetAppEntityByWechatMpAppId(string wechat_mp_appid)
    {
        MapAppId.TryGetValue(wechat_mp_appid, out var app_entity);
        return app_entity;
    }

    public ConfigApp GetAppEntityByTaptapAppId(string taptap_appid)
    {
        MapAppId.TryGetValue(taptap_appid, out var app_entity);
        return app_entity;
    }

    public ConfigApp GetAppEntityByFacebookAppId(string facebook_appid)
    {
        MapAppId.TryGetValue(facebook_appid, out var app_entity);
        return app_entity;
    }

    public ConfigApp GetAppEntityByGoogleAppId(string google_appid)
    {
        MapAppId.TryGetValue(google_appid, out var app_entity);
        return app_entity;
    }
}