using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DEF.CCenter;

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
        CCenterOptions ccenter_options = null;

        var host = Host.CreateDefaultBuilder(args)
            .UseDEFServiceNode(args, null, (service_builder, config) =>
            {
                service_builder.Configure<CCenterOptions>(config.GetSection(CCenterOptions.Key));

                def_options = config.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
                service_options = config.GetSection(ServiceOptions.Key).Get<ServiceOptions>();
                ccenter_options = config.GetSection(CCenterOptions.Key).Get<CCenterOptions>();

                service_builder.Services.AddHostedService<CCenterContext>();
            })
            .Build();

        await host.RunAsync();

        return 0;
    }
}