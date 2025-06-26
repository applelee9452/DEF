using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DEF.SyncDb;

internal class Program
{
    public static int Main(string[] args)
    {
        return RunMainAsync(args).Result;
    }

    static async Task<int> RunMainAsync(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((service_collection) =>
            {
                service_collection.AddSingleton<DEF.Rpcer4Service>();
                service_collection.AddSingleton<DEF.ServicePubSub>();
                service_collection.AddSingleton<DEF.Service>();
                service_collection.AddSingleton<TestContext>();
                service_collection.AddSingleton<TestEntity>();
                service_collection.AddHostedService<TestService>();
            })
            .UseSerilog()
            .Build();

        await host.StartAsync();

        while (true)
        {
            Console.Write("[applelee]: ");

            string s = Console.ReadLine();

            if (s == "q" || s == "exit" || s == null)
            {
                break;
            }

            string[] cmd_args = s.Split(' ');
            if (cmd_args == null || cmd_args.Length == 0)
            {
                continue;
            }

            string cmd = cmd_args[0];
            if (string.IsNullOrEmpty(cmd))
            {
                continue;
            }

            var cmd_args2 = cmd_args.Take(new Range(1, Index.End));

            if (cmd == "addentity")
            {
                //await TestContext.Instance.Test1();
            }
            else if (cmd == "deleteentity")
            {
                //await TestContext.Instance.Test2();
            }

            //Parser.Default.ParseArguments<CmdlineOptions>(cmd_args2)
            //    .WithParsed(options =>
            //    {
            //        var parsed = (CmdlineOptions)options;
            //        var s = parsed.ConfigFileName;

            //        Console.WriteLine($"ConfigFileName: {s}");
            //    })
            //    .WithNotParsed(errors =>
            //    {
            //        foreach (var error in errors)
            //        {
            //            Console.WriteLine(error.ToString());
            //        }
            //    });
        }

        await host.StopAsync();

        return 0;
    }
}