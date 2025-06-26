using Microsoft.Extensions.Options;
using SuperSocket;
using SuperSocket.Channel;
using SuperSocket.Server;
using SuperSocket.Server.Abstractions;
using SuperSocket.Server.Abstractions.Session;

namespace DEF.Gateway;

public class SuperSocketServerHostedService : SuperSocketService<SuperSocketPacketInfo>
{
    ILogger Logger2 { get; set; }
    IOptions<DEFOptions> DEFOptions { get; set; }
    IOptions<GatewayOptions> GatewayOptions { get; set; }

    public SuperSocketServerHostedService(
        IServiceProvider serviceProvider,
        IOptions<ServerOptions> serverOptions,
        ILogger<SuperSocketServerHostedService> logger,
        IOptions<DEFOptions> def_options,
        IOptions<GatewayOptions> gateway_options)
        : base(serviceProvider, serverOptions)
    {
        Logger2 = logger;
        DEFOptions = def_options;
        GatewayOptions = gateway_options;
    }

    protected override async ValueTask OnSessionConnectedAsync(IAppSession session)
    {
        await base.OnSessionConnectedAsync(session);
    }

    protected override async ValueTask OnSessionClosedAsync(IAppSession session, SuperSocket.Connection.CloseEventArgs e)
    {
        await base.OnSessionClosedAsync(session, e);
    }

    protected override ValueTask OnStartedAsync()
    {
        Logger2.LogInformation("SuperSocketServer Tcp启动成功，Ip=0.0.0.0，Port={Port}",
            Options.Listeners[0].Port);

        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnStopAsync()
    {
        Logger2.LogInformation("SuperSocketServer 停止成功！");

        return ValueTask.CompletedTask;
    }
}