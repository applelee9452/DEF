using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DEF;

// 已注入，但未使用
public class ServiceNodeAssembly
{
    ILogger Logger { get; set; }
    IServiceProvider ServiceProvider { get; set; }
    IStartup Startup { get; set; }

    public ServiceNodeAssembly(ILogger<OrleansStartup> logger,
        IServiceProvider service_provider)
    {
        Logger = logger;
        ServiceProvider = service_provider;
    }

    public Task Start(string assembly_fullname, string startup_type)
    {
        Assembly ass = Assembly.LoadFrom(assembly_fullname);
        Type type = ass.GetType(startup_type, true);
        Startup = Activator.CreateInstance(type) as IStartup;
        return Startup.Start(Logger, ServiceProvider);
    }

    public Task Run(IGrainFactory grain_factory)
    {
        if (Startup != null)
        {
            return Startup.Run(grain_factory);
        }

        return Task.CompletedTask;
    }

    public Task Stop()
    {
        if (Startup != null)
        {
            return Startup.Stop();
        }

        return Task.CompletedTask;
    }
}