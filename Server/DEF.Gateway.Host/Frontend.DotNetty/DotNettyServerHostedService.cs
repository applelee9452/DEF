

using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DEF.Gateway;

public class DotNettyServerHostedService : IHostedService
{
    readonly ILogger Logger;
    readonly IOptions<DEFOptions> DEFOptions;
    readonly IOptions<GatewayOptions> GatewayOptions;
    readonly MultithreadEventLoopGroup BossGroup = new(1);
    readonly MultithreadEventLoopGroup WorkerGroup = new(4);
    readonly ServerBootstrap Bootstrap = new();
    IChannel BootstrapChannel = null;

    public DotNettyServerHostedService(
        ILogger<DotNettyServerHostedService> logger,
        IOptions<DEFOptions> def_options,
        IOptions<GatewayOptions> gateway_options)
    {
        Logger = logger;
        DEFOptions = def_options;
        GatewayOptions = gateway_options;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Bootstrap
            .Group(BossGroup, WorkerGroup)
            .Channel<TcpServerSocketChannel>()
            //.Option(ChannelOption.SoBacklog, 4096)
            .Option(ChannelOption.SoKeepalive, true)
            .Option(ChannelOption.TcpNodelay, true)
            .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
            {
                IChannelPipeline pipeline = channel.Pipeline;
                pipeline.AddLast(new LengthFieldPrepender(
                    ByteOrder.LittleEndian, 4, 0, false));
                pipeline.AddLast(new LengthFieldBasedFrameDecoder(
                    ByteOrder.LittleEndian, 1024 * 1024, 0, 4, 0, 4, false));
                pipeline.AddLast(new DotNettyChannelHandler());
            }));

        BootstrapChannel = await Bootstrap.BindAsync(GatewayOptions.Value.ListenPortTcp);// GatewayIp

        Logger.LogInformation("DotNettyServer Tcp启动成功，Ip=0.0.0.0，Port={Port}",
            GatewayOptions.Value.ListenPortTcp);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await BootstrapChannel.CloseAsync();
        }
        finally
        {
            await Task.WhenAll(
                BossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                WorkerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
        }

        Logger.LogInformation("DotNettyServer Tcp停止成功！");
    }
}