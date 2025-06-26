using SuperSocket;
using SuperSocket.Server;
using SuperSocket.Server.Abstractions;
using SuperSocket.Server.Host;
using System.Net;
using System.Text;

namespace DEF.Gateway;

internal class Program
{
    static Thread ThreadWait4Shutdown { get; set; }

    static volatile bool Close = false;

    public static int Main(string[] args)
    {
        return RunMainAsync(args).Result;
    }

    static async Task<int> RunMainAsync(string[] args)
    {
        var config_root = ServiceNodeGenericHostExtensions.BuildConfig(args);

        DEFOptions def_options = config_root.GetRequiredSection(DEFOptions.Key).Get<DEFOptions>();
        ServiceOptions service_options = config_root.GetSection(ServiceOptions.Key).Get<ServiceOptions>();
        ServiceClientOptions serviceclient_options = config_root.GetSection(ServiceClientOptions.Key).Get<ServiceClientOptions>();
        GatewayOptions gateway_options = config_root.GetSection(GatewayOptions.Key).Get<GatewayOptions>();

        var host = Host.CreateDefaultBuilder(args)
            .UseDEFServiceClient(args, new ServiceClientObserverListener(), (serviceclient_builder, config) =>
            {
            })
            .UseDEFServiceNode(args, null, (service_builder, config) =>
            {
                service_builder.Configure<GatewayOptions>(config.GetSection(GatewayOptions.Key));

                service_builder.Services.AddSingleton<GatewayService>();
                service_builder.Services.AddHostedService<GatewayContext>();

                //if (gateway_options.TcpServer == "DotNetty")
                //{
                //    service_builder.Services.AddHostedService<DotNettyServerHostedService>();
                //}
                //else if (gateway_options.TcpServer == "Kcp")
                //{
                //    service_builder.Services.AddHostedService<KcpServerHostedService>();
                //}
            })
            .UseSuperSocket((host_builder) =>
            {
                gateway_options = config_root.GetSection(GatewayOptions.Key).Get<GatewayOptions>();

                if (gateway_options.TcpServer != "SuperSocket")
                {
                    return null;
                }

                var builder2 = host_builder.AsSuperSocketHostBuilder<SuperSocketPacketInfo, SuperSocketPipelineFilter>()
                .ConfigureSuperSocket(options =>
                {
                    options.Name = "SuperSocketServer";
                    options.DefaultTextEncoding = Encoding.UTF8;
                    options.ClearIdleSessionInterval = 120;// 毫秒
                    options.IdleSessionTimeOut = 300;// 毫秒
                    options.ReceiveBufferSize = 8192;
                    options.SendBufferSize = 8192;
                    options.AddListener(new ListenOptions
                    {
                        Ip = "Any",
                        Port = gateway_options.ListenPortTcp,
                        NoDelay = true,
                        AuthenticationOptions = new()
                        {
                            EnabledSslProtocols = System.Security.Authentication.SslProtocols.None,
                            CertificateOptions = new()
                            {
                                FilePath = "gateway.pfx",
                                Password = "h%j*Qd3Vy"
                            }
                        }
                    });
                })
                .UseSession<SuperSocketChannelHandler>()
                .UsePackageHandler(
                (session, packet) =>
                {
                    var s = (SuperSocketChannelHandler)session;
                    return s.OnRecvPackage(packet);
                },
                (session, h) =>
                {
                    var s = (SuperSocketChannelHandler)session;
                    return s.OnRecvError(h);
                })
                .UseHostedService<SuperSocketServerHostedService>();

                return builder2;
            })
            .ConfigureWebHostDefaults(web_builder =>
            {
                web_builder
                .ConfigureServices((hostbuilder_context, services) =>
                {
                    services.AddControllers();
                    services.AddSignalR();
                    services.AddGrpc();
                    services.AddHttpClient();
                    services.AddCors(options =>
                    {
                        options.AddDefaultPolicy(policy =>
                        {
                            policy.AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowAnyOrigin();
                        });
                        //options.AddPolicy("h5", policy =>
                        //{
                        //    policy.AllowAnyHeader()
                        //        .AllowAnyMethod()
                        //        //.AllowCredentials()
                        //        .AllowAnyOrigin();
                        //});
                    });
                })
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, gateway_options.ListenPortHttp);
                    options.Listen(IPAddress.Any, gateway_options.ListenPortHttps, listenOptions => { listenOptions.UseHttps(gateway_options.SslFileName, gateway_options.SslPwd); });
                })
                .Configure((WebHostBuilderContext hostbuilder_context, IApplicationBuilder app) =>
                {
                    if (hostbuilder_context.HostingEnvironment.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    else
                    {
                        app.UseExceptionHandler("/Error");
                        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                        app.UseHsts();
                    }

                    //app.UseHttpsRedirection();
                    app.UseRouting();
                    app.UseAuthorization();
                    app.UseCors();
                    //app.UseCors("h5");
                    //app.UseStaticFiles();
                    //app.UseCookiePolicy();

                    var websocket_options = new WebSocketOptions()
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(120),
                    };
                    app.UseWebSockets(websocket_options);

                    //app.UseSignalR(routes =>
                    //{
                    //    routes.MapHub<SignalIRHub>("/hub");
                    //});

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                        endpoints.MapHub<SignalRHub>("/hub");
                        //endpoints.MapGrpcService<GatewayNotifyService>();
                    });
                });
            })
            .Build();

        //await host.RunAsync();

        await host.StartAsync();

        TickMgr tick_mgr = new();

        if (gateway_options.TcpServer == "Kcp")
        {
            tick_mgr.UseKcp(new KcpServerService());
        }

        await tick_mgr.StartAsync();

        ThreadWait4Shutdown = new Thread((o) =>
        {
            host.WaitForShutdown();

            Close = true;
        });
        ThreadWait4Shutdown.Start();

        while (!Close)
        {
            await tick_mgr.Update(0);

            Thread.Sleep(1);
        }

        await tick_mgr.StopAsync();

        if (ThreadWait4Shutdown != null)
        {
            ThreadWait4Shutdown.Join();
            ThreadWait4Shutdown = null;
        }

        //await host.StopAsync();

        return 0;
    }
}