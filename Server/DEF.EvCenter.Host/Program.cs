using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DEF.EvCenter;

internal class Program
{
    public static int Main(string[] args)
    {
        return RunMainAsync(args).Result;
    }

    private static async Task<int> RunMainAsync(string[] args)
    {
        DEFOptions def_options = null;
        ServiceOptions service_options = null;
        EvCenterOptions evcenter_options = null;

        var host = Host.CreateDefaultBuilder(args)
            .UseDEFServiceNode(args, null, (service_builder, config) =>
            {
                service_builder.Configure<EvCenterOptions>(config.GetSection(EvCenterOptions.Key));

                def_options = config.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
                service_options = config.GetSection(ServiceOptions.Key).Get<ServiceOptions>();
                evcenter_options = config.GetSection(EvCenterOptions.Key).Get<EvCenterOptions>();

                service_builder.Services.AddHostedService<EvCenterContext>();
            })
            .Build();

        await host.RunAsync();

        return 0;
    }
}