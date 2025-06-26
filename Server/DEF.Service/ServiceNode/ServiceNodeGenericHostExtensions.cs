using CommandLine;
using DEF;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Serialization;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using SkyApm.Agent.Hosting;
using SkyApm.Agent.GeneralHost;
using StackExchange.Redis;
using Serilog;
using Serilog.Events;
using System.Text.Json;
using System.Runtime.InteropServices;

namespace Microsoft.Extensions.Hosting;

internal class ServiceNodeBuilderMarker
{
    public ServiceNodeBuilderMarker(object builderInstance) => Instance = builderInstance;
    public object Instance { get; }
}

public class CmdlineOptions
{
    [Option('c', "config_filename", Required = false, HelpText = "服务关联的配置文件名")]
    public string ConfigFileName { get; set; }

    [Option('s', "solution_dir", Required = false, HelpText = "解决方案目录名")]
    public string SolutionDir { get; set; }
}

public static class ServiceNodeGenericHostExtensions
{
    public static IConfigurationRoot BuildConfig(string[] args)
    {
        byte[] data_def_json = null;
        byte[] data_service_json = null;

        string dir_prefix = "../";
        string filename1 = "Config/DEF.json";
        string filename2 = string.Empty;

        CmdlineOptions cmd_options = null;
        var cmdline_parseresult = Parser.Default.ParseArguments<CmdlineOptions>(args);
        if (cmdline_parseresult.Tag == ParserResultType.Parsed)
        {
            var parsed = (Parsed<CmdlineOptions>)cmdline_parseresult;
            cmd_options = parsed.Value;

            if (!string.IsNullOrEmpty(cmd_options.ConfigFileName))
            {
                filename2 = string.Format("Config/{0}.json", cmd_options.ConfigFileName);
            }
        }

        if (string.IsNullOrEmpty(filename2))
        {
            filename2 = string.Format("Config/{0}.json", AppDomain.CurrentDomain.FriendlyName);
        }

        string path = AppDomain.CurrentDomain.BaseDirectory;
        if (cmd_options != null && !string.IsNullOrEmpty(cmd_options.SolutionDir))
        {
            path = cmd_options.SolutionDir;
        }

        for (int i = 0; i < 8; i++)
        {
            string p1 = Path.Combine(path, filename1);
            string p2 = Path.GetFullPath(p1);
            string p3 = Path.Combine(path, filename2);
            string p4 = Path.GetFullPath(p3);

            if (File.Exists(p2))
            {
                var str = File.ReadAllText(p2);
                data_def_json = System.Text.Encoding.UTF8.GetBytes(str);

                if (!string.IsNullOrEmpty(filename2) && File.Exists(p4))
                {
                    var str2 = File.ReadAllText(p4);
                    data_service_json = System.Text.Encoding.UTF8.GetBytes(str2);
                }

                break;
            }

            filename1 = dir_prefix + filename1;
            if (!string.IsNullOrEmpty(filename2))
            {
                filename2 = dir_prefix + filename2;
            }
        }

        var builder_config = new ConfigurationBuilder();
        if (data_def_json != null)
        {
            builder_config.AddJsonStream(new MemoryStream(data_def_json));
        }
        if (data_service_json != null)
        {
            builder_config.AddJsonStream(new MemoryStream(data_service_json));
        }
        builder_config.AddCommandLine(args);
        builder_config.AddEnvironmentVariables();
        var config = builder_config.Build();

        return config;
    }

    public static IHostBuilder UseDEFServiceNode(
        this IHostBuilder host_builder,
        string[] args,
        IServiceListener listener,
        Action<IServiceNodeBuilder, IConfigurationRoot> configure_delegate)
    {
        if (host_builder is null) throw new ArgumentNullException(nameof(host_builder));
        if (configure_delegate == null) throw new ArgumentNullException(nameof(configure_delegate));

        // 初始化配置文件
        var config = BuildConfig(args);
        var def_options = config.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
        var service_options = config.GetRequiredSection(ServiceOptions.Key).Get<ServiceOptions>();
        var serviceclient_options = config.GetSection(ServiceClientOptions.Key).Get<ServiceClientOptions>();

        // 控制台标题
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.Title = service_options.ServiceName;
        }

        var local_ipadress = DEF.Utils.GetLocalIpAddress(def_options.LocalIpPrefix);// 获取本机内网IpAddress
        int orleans_gateway_port = service_options.OrleansGatewayPort;
        int orleans_silo_port = service_options.OrleansSiloPort;

        host_builder
        .ConfigureHostConfiguration(config_builder =>
        {
            config_builder.AddConfiguration(config);
        })
        .UseSerilog((host_builder_context, log_cfg) =>
        {
            def_options = host_builder_context.Configuration.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
            //guandan_options = host_builder_context.Configuration.GetSection(GuandanOptions.Key).Get<GuandanOptions>();

            if (def_options.LogEnableOpenObserve)
            {
                log_cfg
                .MinimumLevel.Is(def_options.LogMinimumLevel)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogEventLevel.Warning)
                //.MinimumLevel.Override("Microsoft.Orleans.Messaging", LogEventLevel.Warning)
                .MinimumLevel.Override("Orleans", LogEventLevel.Warning)
                //.MinimumLevel.Override("Runtime", LogEventLevel.Warning)
                //.WriteTo.OpenTelemetry("http://192.168.1.100:5005");
                //.WriteTo.MongoDBCapped(def_options.LogMongoDBConnectString + "/" + def_options.LogMongoDBName,
                //    collectionName: guandan_options.LogFileName, cappedMaxSizeMb: 100, cappedMaxDocuments: 1000000)
                //.WriteTo.OpenTelemetry()
                .WriteTo.OpenObserve(def_options.OpenObserveUrl, def_options.Cluster, def_options.OpenObserveLogin, def_options.OpenObserveKey, service_options.ServiceName)
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            }
            else
            {
                log_cfg
                .MinimumLevel.Is(def_options.LogMinimumLevel)
                .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogEventLevel.Warning)
                //.MinimumLevel.Override("Microsoft.Orleans.Messaging", LogEventLevel.Warning)
                .MinimumLevel.Override("Orleans", LogEventLevel.Warning)
                //.MinimumLevel.Override("Runtime", LogEventLevel.Warning)
                //.WriteTo.OpenTelemetry("http://192.168.1.100:5005");
                .WriteTo.MongoDBCapped(def_options.LogMongoDBConnectString + "/" + def_options.LogMongoDBName,
                    restrictedToMinimumLevel: LogEventLevel.Warning,
                    collectionName: service_options.ServiceName,
                    cappedMaxSizeMb: 100,
                    cappedMaxDocuments: 1000000)
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            }
        })
        .UseOrleans((host_builder_context, silo_builder) =>
        {
            silo_builder
                .Configure<DEFOptions>(config.GetSection(DEFOptions.Key))
                .Configure<ServiceOptions>(config.GetSection(ServiceOptions.Key))
                .Configure<ServiceClientOptions>(config.GetSection(ServiceClientOptions.Key))
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = service_options.ServiceName;
                    options.ServiceId = def_options.Cluster + def_options.Env;
                })
                .Configure<EndpointOptions>(options =>
                {
                    options.AdvertisedIPAddress = local_ipadress;
                    options.GatewayPort = orleans_gateway_port;
                    options.SiloPort = orleans_silo_port;
                })
                .Configure<SiloMessagingOptions>(options =>
                {
                    options.ResponseTimeout = TimeSpan.FromSeconds(10);
                    options.ResponseTimeoutWithDebugger = TimeSpan.FromSeconds(def_options.ResponseTimeoutWithDebugger);
                })
                .Configure<ClientMessagingOptions>(options =>
                {
                    options.ResponseTimeout = TimeSpan.FromSeconds(10);
                    options.ResponseTimeoutWithDebugger = TimeSpan.FromSeconds(def_options.ResponseTimeoutWithDebugger);
                })
                //.Configure<StatisticsOptions>(options =>
                //{
                //    options.CollectionLevel = StatisticsLevel.Critical;
                //})
                //.Configure<SchedulingOptions>(options =>
                //{
                //    options.PerformDeadlockDetection = true;
                //    options.AllowCallChainReentrancy = false;
                //})
                //.ConfigureServices((host_context, services) =>
                //{
                //    services.AddMemoryCache();
                //    services.AddHttpClient();
                //    //services.AddSingleton<Service>();
                //    //services.AddSingleton<ServiceAssembly>();
                //})
                //.ConfigureApplicationParts(_ =>
                //{
                //    _.AddApplicationPart(typeof(IGrainContainerStateful).Assembly).WithReferences();
                //    _.AddApplicationPart(typeof(ContainerStateful).Assembly).WithReferences();
                //})
                .AddMemoryGrainStorageAsDefault()
                .AddMemoryGrainStorage("PubSubStore")
                .AddMemoryStreams("StreamProvider")
                //.AddSimpleMessageStreamProvider("SMSProvider")
                .AddStartupTask<OrleansStartup>(10001)
                //.AddGrainService<Test.CfgService>()  // Register GrainService
                //.ConfigureServices(s =>
                //{
                //    s.AddSingleton<Test.ICfgServiceClient, Test.CfgServiceClient>();
                //})
                .UseTransactions()
                .UseInMemoryReminderService()
                //.UseSiloUnobservedExceptionsHandler()
                //.UseLinuxEnvironmentStatistics()
                //.UsePerfCounterEnvironmentStatistics()
                //.UseDashboard(options =>
                //{
                //    options.Host = "*";
                //    options.Port = 10052;
                //})
                //.UseDashboard(options =>
                //{
                //    options.Username = "admin";
                //    options.Password = "99890618";
                //    options.Host = "*";
                //    options.Port = 8001;
                //    options.HostSelf = true;
                //    options.HideTrace = true;
                //    options.CounterUpdateIntervalMs = 5000;
                //})
                //.UseLocalhostClustering();
                //.UseZooKeeperClustering(options =>
                //{
                //    options.ConnectionString = def_options.ZkConnectString;
                //});
                .UseRedisClustering(options =>
                {
                    string redis_connection_string = def_options.RedisConnectString;
                    options.ConfigurationOptions = ConfigurationOptions.Parse(redis_connection_string);
                });
        });

        return host_builder.ConfigureServices((context, services) =>
        {
            services.AddDEFServiceNode(config, listener, service_options.ServiceName, configure_delegate);
        });
    }

    public static IServiceCollection AddDEFServiceNode(
        this IServiceCollection services,
        IConfigurationRoot config,
        IServiceListener listener,
        string service_name,
        Action<IServiceNodeBuilder, IConfigurationRoot> configureDelegate)
    {
        if (configureDelegate == null) throw new ArgumentNullException(nameof(configureDelegate));

        IServiceNodeBuilder builder = default;

        foreach (var descriptor in services)
        {
            if (descriptor.ServiceType.Equals(typeof(ServiceNodeBuilderMarker)))
            {
                var instance = (ServiceNodeBuilderMarker)descriptor.ImplementationInstance;
                builder = (IServiceNodeBuilder)instance.Instance;
                //switch
                //{
                //    IDEFServiceBuilder existingBuilder => existingBuilder,
                //    _ => throw GetOrleansClientAddedException()
                //};
            }
        }

        if (builder is null)
        {
            builder = new ServiceNodeBuilder(services, service_name);
            services.Add(new(typeof(ServiceNodeBuilderMarker), new ServiceNodeBuilderMarker(builder)));
        }

        if (listener != null)
        {
            services.AddSingleton<IServiceListener>(sp =>
            {
                var s = sp.GetService<Service>();
                listener.Service = s;
                return listener;
            });
        }
        else
        {
            services.AddSingleton<IServiceListener>(new ServiceListenerDefault());
        }

        configureDelegate(builder, config);

        return services;
    }
}