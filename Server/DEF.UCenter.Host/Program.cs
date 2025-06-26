using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Senparc.CO2NET.RegisterServices;

namespace DEF.UCenter;

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
        UCenterOptions ucenter_options = null;

        var host = Host.CreateDefaultBuilder(args)
            .UseDEFServiceNode(args, null, (service_builder, config) =>
            {
                service_builder.Configure<UCenterOptions>(config.GetSection(UCenterOptions.Key));

                def_options = config.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
                service_options = config.GetSection(ServiceOptions.Key).Get<ServiceOptions>();
                ucenter_options = config.GetSection(UCenterOptions.Key).Get<UCenterOptions>();

                service_builder.Services.AddSenparcGlobalServices(config);
                service_builder.Services.AddHostedService<UCenterContext>();
            })
            .Build();

        await host.RunAsync();

        return 0;
    }
}