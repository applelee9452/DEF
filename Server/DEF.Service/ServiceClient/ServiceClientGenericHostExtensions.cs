using DEF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Microsoft.Extensions.Hosting;

internal class ServiceClientBuilderMarker
{
    public ServiceClientBuilderMarker(object builderInstance) => Instance = builderInstance;

    public object Instance { get; }
}

public static class ServiceClientGenericHostExtensions
{
    public static IHostBuilder UseDEFServiceClient(
        this IHostBuilder host_builder,
        string[] args,
        IServiceClientObserverListener listener,
        Action<IServiceClientBuilder, IConfigurationRoot> configure_delegate)
    {
        if (host_builder is null) throw new ArgumentNullException(nameof(host_builder));
        if (configure_delegate == null) throw new ArgumentNullException(nameof(configure_delegate));

        // 初始化配置文件
        var config = ServiceNodeGenericHostExtensions.BuildConfig(args);
        var def_options = config.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
        var service_options = config.GetSection(ServiceOptions.Key).Get<ServiceOptions>();
        var serviceclient_options = config.GetSection(ServiceClientOptions.Key).Get<ServiceClientOptions>();

        host_builder
            .ConfigureHostConfiguration(config_builder =>
            {
                config_builder.AddConfiguration(config);
            });
        //.UseSerilog((host_builder_context, log_cfg) =>
        //{
        //    //def_options = host_builder_context.Configuration.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
        //    //guandan_options = host_builder_context.Configuration.GetSection(GuandanOptions.Key).Get<GuandanOptions>();

        //    log_cfg
        //        .MinimumLevel.Information()
        //        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
        //        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", LogEventLevel.Warning)
        //        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogEventLevel.Warning)
        //        .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogEventLevel.Warning)
        //        //.MinimumLevel.Override("Microsoft.Orleans.Messaging", LogEventLevel.Warning)
        //        //.MinimumLevel.Override("Orleans", LogEventLevel.Warning)
        //        //.MinimumLevel.Override("Runtime", LogEventLevel.Warning)
        //        //.WriteTo.OpenTelemetry("http://192.168.1.100:5005");
        //        //.WriteTo.MongoDBCapped(def_options.LogMongoDBConnectString + "/" + def_options.LogMongoDBName,
        //        //    collectionName: guandan_options.LogFileName, cappedMaxSizeMb: 100, cappedMaxDocuments: 1000000)
        //        .WriteTo.OpenTelemetry()
        //        .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        //});

        return host_builder.ConfigureServices((context, services) =>
        {
            services.AddDEFServiceClient(config, listener, "Client", configure_delegate);
        });
    }

    public static WebApplicationBuilder UseWebApplicationServiceClient(
        this WebApplicationBuilder host_builder,
        string[] args,
        IServiceClientObserverListener listener,
        Action<IServiceClientBuilder, IConfigurationRoot> configure_delegate)
    {
        if (host_builder is null) throw new ArgumentNullException(nameof(host_builder));
        if (configure_delegate == null) throw new ArgumentNullException(nameof(configure_delegate));

        // 初始化配置文件
        var config = ServiceNodeGenericHostExtensions.BuildConfig(args);
        var def_options = config.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
        var service_options = config.GetSection(ServiceOptions.Key).Get<ServiceOptions>();
        var serviceclient_options = config.GetSection(ServiceClientOptions.Key).Get<ServiceClientOptions>();

        //host_builder
        //    .ConfigureHostConfiguration(config_builder =>
        //    {
        //        config_builder.AddConfiguration(config);
        //    });
        //.UseSerilog((host_builder_context, log_cfg) =>
        //{
        //    //def_options = host_builder_context.Configuration.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
        //    //guandan_options = host_builder_context.Configuration.GetSection(GuandanOptions.Key).Get<GuandanOptions>();

        //    log_cfg
        //        .MinimumLevel.Information()
        //        .MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Warning)
        //        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", LogEventLevel.Warning)
        //        .MinimumLevel.Override("Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogEventLevel.Warning)
        //        .MinimumLevel.Override("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogEventLevel.Warning)
        //        //.MinimumLevel.Override("Microsoft.Orleans.Messaging", LogEventLevel.Warning)
        //        //.MinimumLevel.Override("Orleans", LogEventLevel.Warning)
        //        //.MinimumLevel.Override("Runtime", LogEventLevel.Warning)
        //        //.WriteTo.OpenTelemetry("http://192.168.1.100:5005");
        //        //.WriteTo.MongoDBCapped(def_options.LogMongoDBConnectString + "/" + def_options.LogMongoDBName,
        //        //    collectionName: guandan_options.LogFileName, cappedMaxSizeMb: 100, cappedMaxDocuments: 1000000)
        //        .WriteTo.OpenTelemetry()
        //        .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        //});
        host_builder.Configuration.AddConfiguration(config);

        host_builder.Services.AddDEFServiceClient(config, listener, "Client", configure_delegate);

        return host_builder;
    }

    public static IServiceCollection AddDEFServiceClient(
        this IServiceCollection services,
        IConfigurationRoot config,
        IServiceClientObserverListener listener,
        string service_name,
        Action<IServiceClientBuilder, IConfigurationRoot> configure_delegate)
    {
        if (configure_delegate == null) throw new ArgumentNullException(nameof(configure_delegate));

        IServiceClientBuilder builder = default;

        foreach (var descriptor in services)
        {
            if (descriptor.ServiceType.Equals(typeof(ServiceClientBuilderMarker)))
            {
                var instance = (ServiceClientBuilderMarker)descriptor.ImplementationInstance;
                builder = (IServiceClientBuilder)instance.Instance;
                break;
            }
        }

        if (builder is null)
        {
            builder = new ServiceClientBuilder(services, service_name);
            services.Add(new(typeof(ServiceClientBuilderMarker), new ServiceClientBuilderMarker(builder)));
        }

        if (listener != null)
        {
            services.AddSingleton<IServiceClientObserverListener>(listener);
        }
        else
        {
            services.AddSingleton<IServiceClientObserverListener>(new ServiceClientObserverListenerDefault());
        }

        builder
            .Configure<DEFOptions>(config.GetRequiredSection(DEFOptions.Key))
            .Configure<ServiceOptions>(config.GetSection(ServiceOptions.Key))
            .Configure<ServiceClientOptions>(config.GetSection(ServiceClientOptions.Key));

        configure_delegate(builder, config);

        return services;
    }
}