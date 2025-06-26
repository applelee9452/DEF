using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DEF;

internal class ServiceNodeHostedService : IHostedService
{
    ILogger Logger { get; set; }
    ServiceNode ServiceNode { get; set; }

    public ServiceNodeHostedService(ServiceNode service_node, ILogger<ServiceNodeHostedService> logger)
    {
        ServiceNode = service_node;
        Logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ServiceNode.StartAsync(cancellationToken);

        Logger.LogInformation("ServiceNode启动成功！");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await ServiceNode.StopAsync(cancellationToken);

        Logger.LogInformation("ServiceNode停止成功！");
    }
}