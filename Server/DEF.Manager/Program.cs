using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Runtime.InteropServices;

namespace DEF.Manager;

internal class Program
{
    public static void Main(string[] args)
    {
        IConfigurationRoot config_root = null;
        DEFOptions def_options = null;
        ManagerOptions manager_options = null;

        var builder = WebApplication.CreateBuilder(args)
            .UseWebApplicationServiceClient(args, null, (serviceclient_builder, config) =>
            {
                serviceclient_builder.Configure<ManagerOptions>(config.GetSection(ManagerOptions.Key));

                config_root = config;
                def_options = config.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
                manager_options = config.GetSection(ManagerOptions.Key).Get<ManagerOptions>();

                var plugins = new ManagerPlugins(serviceclient_builder.Services, manager_options);
                serviceclient_builder.Services.AddSingleton<ManagerDb>();
                serviceclient_builder.Services.AddSingleton<ManagerContext>();
                serviceclient_builder.Services.AddSingleton(typeof(ManagerPlugins), plugins);
                serviceclient_builder.Services.AddScoped<ManagerSession>();
                serviceclient_builder.Services.AddHostedService<ManagerService>();

                // 控制台标题
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Console.Title = ManagerOptions.Key;
                }
            });

        builder.Services.AddResponseCompression();
        builder.Services.AddRazorPages();
        builder.Services.AddRazorComponents().AddInteractiveServerComponents();
        builder.Services.AddBootstrapBlazor();
        builder.Services.AddBootstrapBlazorTableExportService();
        builder.Services.AddScoped<AuthenticationStateProvider, ManagerAuthenticationStateProvider>();
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
        builder.Services.AddAuthorization();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddBlazoredLocalStorage();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, manager_options.ListenPort, listenOptions => { });

            //#if DEBUG
            //            options.Listen(IPAddress.Any, manager_options.ListenPort, listenOptions => { listenOptions.UseHttps("localhost.pfx", "123456"); });
            //#else
            //            options.Listen(IPAddress.Any, manager_options.ListenPort, listenOptions => { listenOptions.UseHttps(manager_options.SslFileName, manager_options.SslPwd); });
            //#endif
        });

        var app = builder.Build();

        app.UseStatusCodePages(context =>
        {
            if (context.HttpContext.Response.StatusCode == 404)
            {
                context.HttpContext.Response.Redirect("/");
            }
            return Task.CompletedTask;
        });

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseResponseCompression();
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapDefaultControllerRoute();
        app.MapRazorComponents<Shared.App>().AddInteractiveServerRenderMode();

        app.Run();
    }
}