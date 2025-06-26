using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DEF;

internal class ServiceClientHostedService : IHostedService
{
    ILogger Logger { get; set; }
    ServiceClient ServiceClient { get; set; }

    public ServiceClientHostedService(ServiceClient service_client, ILogger<ServiceClientHostedService> logger)
    {
        ServiceClient = service_client;
        Logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ServiceClient.StartAsync(cancellationToken);

        Logger.LogInformation("ServiceClientHostedService�����ɹ���");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await ServiceClient.StopAsync(cancellationToken);

        Logger.LogInformation("ServiceClientHostedServiceֹͣ�ɹ���");
    }
}