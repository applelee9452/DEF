using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DEF;

internal class ServiceDiscoverHostedService : IHostedService
{
    ILogger Logger { get; set; }
    ServiceDiscover ServiceDiscover { get; set; }

    public ServiceDiscoverHostedService(ServiceDiscover service_discover, ILogger<ServiceDiscoverHostedService> logger)
    {
        ServiceDiscover = service_discover;
        Logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ServiceDiscover.StartAsync();

        Logger.LogInformation("ServiceDiscover启动成功！");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await ServiceDiscover.StopAsync();

        Logger.LogInformation("ServiceDiscover停止成功！");
    }
}