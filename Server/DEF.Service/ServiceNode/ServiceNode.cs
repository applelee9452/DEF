using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DEF;

public class ServiceNode
{
    public Func<Task> FuncOrleansStartupExcute;
    public readonly IOptions<DEFOptions> DEFOptions;
    public readonly IOptions<ServiceOptions> ServiceOptions;
    public readonly ILogger Logger;
    public readonly Service Service;
    public readonly Rpcer4Service Rpcer;

    public ServiceNode(ILogger<ServiceNode> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ServiceOptions> service_options,
        Service service,
        Rpcer4Service rpcer)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceOptions = service_options;
        Service = service;
        Rpcer = rpcer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}