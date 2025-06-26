using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DEF;

internal class ServicePubSubHostedService : IHostedService
{
    ILogger Logger { get; set; }
    ServicePubSub ServicePubSub { get; set; }

    public ServicePubSubHostedService(ServicePubSub service_pubsub, ILogger<ServicePubSubHostedService> logger)
    {
        ServicePubSub = service_pubsub;
        Logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellation_token)
    {
        await ServicePubSub.StartAsync();

        Logger.LogInformation("ServicePubSub启动成功！");
    }

    public async Task StopAsync(CancellationToken cancellation_token)
    {
        await ServicePubSub.StopAsync();

        Logger.LogInformation("ServicePubSub停止成功！");
    }
}