using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF;

internal class ServiceNodeAdded { }

internal class ServiceNodeBuilder : IServiceNodeBuilder
{
    public IServiceCollection Services { get; }

    private static readonly ServiceDescriptor ServiceDescriptor = new(typeof(ServiceNodeAdded), new ServiceNodeAdded());

    public ServiceNodeBuilder(IServiceCollection services, string service_name)
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
        Services.TryAddSingleton<ServiceNode>();
        //Services.TryAddSingleton<ServiceNodeAssembly>();
        Services.TryAddSingleton<ServiceDiscover>();
        Services.TryAddSingleton<ServicePubSub>();
        Services.TryAddSingleton<IContainerFactory>(new ContainerFactory());
        Services.AddSingleton<ICommand, DefaultCommand>();
        Services.AddHostedService<ServiceNodeHostedService>();
        Services.AddHostedService<ServiceDiscoverHostedService>();
        Services.AddHostedService<ServicePubSubHostedService>();

        Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.Clear();
                resource.AddService(service_name);
            })
            //.WithLogging(logs => logs.AddConsoleExporter().AddOtlpExporter(op =>
            //{
            //    op.Endpoint = new Uri("http://localhost:4317");
            //    op.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            //}))
            .WithTracing(tracing => tracing.AddConsoleExporter().AddOtlpExporter(op =>
            {
                op.Endpoint = new Uri("http://localhost:4317");
                op.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            }))
            .WithMetrics(metrics => metrics.AddConsoleExporter().AddOtlpExporter(op =>
            {
                op.Endpoint = new Uri("http://localhost:4317");
                op.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            }));
    }
}