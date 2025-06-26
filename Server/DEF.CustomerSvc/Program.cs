using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DEF.CustomerSvc;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(options =>
                {
                    options.Listen(System.Net.IPAddress.Any, 80);
                    //options.Listen(IPAddress.Any, 443, listen_options =>
                    //{
                    //    //listen_options.UseHttps(Casinos.Config.ConfigManager.SslFileName, Casinos.Config.ConfigManager.SslPwd);
                    //});
                });
                webBuilder.UseStartup<Startup>();
            });
}