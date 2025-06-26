using DEF.Gateway;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DEF;

internal class ServiceNodeClientAdded
{
}

internal class ServiceClientBuilder : IServiceClientBuilder
{
    public IServiceCollection Services { get; }
    static ServiceDescriptor ServiceDescriptor { get; set; } = new(typeof(ServiceNodeClientAdded), new ServiceNodeClientAdded());

    public ServiceClientBuilder(IServiceCollection services, string service_name)
    {
        if (services.Contains(ServiceDescriptor))
        {
            return;
        }

        Services = services;

        Services.Add(ServiceDescriptor);

        ServicesAdd.AddMemoryCache(Services);
        ServicesAdd.AddHttpClient(Services);

        Services.TryAddSingleton<Rpcer4Service>();
        Services.TryAddSingleton<Service>();
        Services.TryAddSingleton<ServiceClient>();
        Services.TryAddSingleton<ServiceDiscover>();
        Services.TryAddSingleton<ServicePubSub>();
        Services.TryAddSingleton<ServiceClientBackgroundService>();
        Services.AddHostedService<ServiceClientHostedService>();
        Services.AddHostedService<ServiceDiscoverHostedService>();
        Services.AddHostedService<ServicePubSubHostedService>();
        Services.AddHostedService<ServiceClientBackgroundService>();

        //Services.AddOpenTelemetry()
        //    .ConfigureResource(resource =>
        //    {
        //        resource.Clear();
        //        resource.AddService(service_name);
        //    })
        //    //.WithLogging(logs => logs.AddConsoleExporter().AddOtlpExporter(op =>
        //    //{
        //    //    op.Endpoint = new Uri("http://localhost:4317");
        //    //    op.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        //    //}))
        //    .WithTracing(tracing => tracing.AddConsoleExporter().AddOtlpExporter(op =>
        //    {
        //        op.Endpoint = new Uri("http://localhost:4317");
        //        op.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        //    }))
        //    .WithMetrics(metrics => metrics.AddConsoleExporter().AddOtlpExporter(op =>
        //    {
        //        op.Endpoint = new Uri("http://localhost:4317");
        //        op.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        //    }));
    }
}