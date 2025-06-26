using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DEF.IM;

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
        IMOptions im_options = null;

        var host = Host.CreateDefaultBuilder(args)
            .UseDEFServiceNode(args, new IMServiceListener(), (service_builder, config) =>
            {
                service_builder.Configure<IMOptions>(config.GetSection(IMOptions.Key));

                def_options = config.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
                service_options = config.GetSection(ServiceOptions.Key).Get<ServiceOptions>();
                im_options = config.GetSection(IMOptions.Key).Get<IMOptions>();

                service_builder.Services.AddHostedService<IMContext>();
            })
            .Build();

        await host.RunAsync();

        return 0;
    }
}