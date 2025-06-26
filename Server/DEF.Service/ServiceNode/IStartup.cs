using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading.Tasks;

namespace DEF;

public interface IStartup
{
    Task Start(ILogger logger, IServiceProvider service_provider);

    Task Run(IGrainFactory grain_factory);

    Task Stop();
}