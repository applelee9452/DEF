

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kcp2k;
using System.Diagnostics;

namespace DEF.Gateway;

public class KcpServerHostedService : IHostedService
{
    ILogger Logger { get; set; }
    IOptions<DEFOptions> DEFOptions { get; set; }
    IOptions<GatewayOptions> GatewayOptions { get; set; }
    Thread ThreadUpdate { get; set; }
    KcpServer KcpServer { get; set; }
    Dictionary<int, KcpChannelHandler> MapChannelHandler { get; set; } = new();
    volatile bool Close = false;

    public KcpServerHostedService(
        ILogger<KcpServerHostedService> logger,
        IOptions<DEFOptions> def_options,
        IOptions<GatewayOptions> gateway_options)
    {
        Logger = logger;
        DEFOptions = def_options;
        GatewayOptions = gateway_options;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation($"KcpServer 启动成功，Ip=0.0.0.0，Port={GatewayOptions.Value.ListenPortTcp}");

        KcpConfig config = new(
            // force NoDelay and minimum interval.
            // this way UpdateSeveralTimes() doesn't need to wait very long and
            // tests run a lot faster.
            NoDelay: true,
            // not all platforms support DualMode.
            // run tests without it so they work on all platforms.
            DualMode: false,
            Interval: 1, // 1ms so at interval code at least runs.
            Timeout: 10000,
            RecvBufferSize: 1024 * 1027 * 7,
            SendBufferSize: 1024 * 1027 * 7,
            FastResend: 2,
            // large window sizes so large messages are flushed with very few
            // update calls. otherwise tests take too long.
            SendWindowSize: Kcp.WND_SND * 1000,
            ReceiveWindowSize: Kcp.WND_RCV * 1000,
            // congestion window _heavily_ restricts send/recv window sizes
            // sending a max sized message would require thousands of updates.
            CongestionWindow: false,
            // maximum retransmit attempts until dead_link detected
            // default * 2 to check if configuration works
            MaxRetransmits: Kcp.DEADLINK * 2
        );

        KcpServer = new(
            OnKcpConnected,
            OnKcpData,
            OnKcpClosed,
            OnKcpError,
            config
        );
        KcpServer.Start((ushort)GatewayOptions.Value.ListenPortTcp);

        ThreadUpdate = new Thread(Update);
        ThreadUpdate.Start();

        return Task.CompletedTask;
    }

    void Update()
    {
        while (!Close)
        {
            KcpServer?.Tick();

            if (!KcpServer.IsActive()) break;

            Thread.Sleep(1);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("KcpServer 停止成功！");

        MapChannelHandler.Clear();

        Close = true;
        if (ThreadUpdate != null)
        {
            ThreadUpdate.Join();
            ThreadUpdate = null;
        }

        if (KcpServer != null)
        {
            KcpServer.Stop();
            KcpServer = null;
        }

        return Task.CompletedTask;
    }

    async void OnKcpData(int connection_id, ArraySegment<byte> data, KcpChannel kcp_channel)
    {
        //Console.WriteLine($"OnKcpData ConnectionId={connection_id} Len={data.Count}");

        if (!KcpServer.connections.ContainsKey(connection_id))
        {
            return;
        }

        if (!MapChannelHandler.TryGetValue(connection_id, out var session))
        {
            return;
        }

        await session.OnRecvPackage(data);
    }

    async void OnKcpConnected(int connection_id)
    {
        //Console.WriteLine($"OnKcpConnected ConnectionId={connection_id}");

        if (!KcpServer.connections.TryGetValue(connection_id, out var connection))
        {
            return;
        }

        if (MapChannelHandler.TryGetValue(connection_id, out var session))
        {
            MapChannelHandler.Remove(connection_id);

            await session.ClosedAsync2();
        }

        var session_new = new KcpChannelHandler();
        MapChannelHandler[connection_id] = session_new;
        session_new.Connected(connection, connection.remoteEndPoint);
    }

    async void OnKcpClosed(int connection_id)
    {
        //Console.WriteLine($"OnKcpClosed ConnectionId={connection_id}");

        if (!KcpServer.connections.ContainsKey(connection_id))
        {
            return;
        }

        if (MapChannelHandler.TryGetValue(connection_id, out var session))
        {
            MapChannelHandler.Remove(connection_id);

            await session.ClosedAsync2();
        }
    }

    async void OnKcpError(int connection_id, kcp2k.ErrorCode error_code, string reason)
    {
        //Console.WriteLine($"OnKcpError ConnectionId={connection_id} ErrorCode={error_code} reason={reason}");

        if (!KcpServer.connections.ContainsKey(connection_id))
        {
            return;
        }

        if (MapChannelHandler.TryGetValue(connection_id, out var session))
        {
            MapChannelHandler.Remove(connection_id);

            await session.ClosedAsync2();
        }
    }
}