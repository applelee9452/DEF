using kcp2k;

namespace DEF.Gateway;

public class KcpServerService : ITickBase
{
    ILogger Logger { get; set; }
    KcpConfig KcpConfig { get; set; }
    KcpServer KcpServer { get; set; }
    Dictionary<int, KcpChannelHandler> MapChannelHandler { get; set; } = new();

    public Task StartAsync()
    {
        Logger = GatewayContext.Instance.Logger;
        var option = GatewayContext.Instance.GatewayOptions.Value;

        int timeout = option.TcpServerTimeout;
#if DEBUG
        timeout = option.TcpServerTimeoutForDebug;
#endif

        KcpConfig = new(
            // force NoDelay and minimum interval.
            // this way UpdateSeveralTimes() doesn't need to wait very long and
            // tests run a lot faster.
            NoDelay: true,
            // not all platforms support DualMode.
            // run tests without it so they work on all platforms.
            DualMode: false,
            Interval: 1, // 1ms so at interval code at least runs.
            Timeout: timeout * 1000,
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
            KcpConfig
        );
        KcpServer.Start((ushort)option.ListenPortTcp);

        Logger.LogInformation($"KcpServer 启动成功，Ip=0.0.0.0，Port={option.ListenPortTcp}");

        return Task.CompletedTask;
    }

    public Task Update(float tm)
    {
        //GatewayContext.Instance.ServiceClient.AllConnected

        var q = GatewayContext.Instance.QueKcpSendData;
        while (!q.IsEmpty)
        {
            if (q.TryDequeue(out var item))
            {
                //Logger.LogInformation($"SendData ThreadId={Thread.CurrentThread.ManagedThreadId}");

                item.Peer.SendData(item.Data, KcpChannel.Reliable);
            }
        }
        KcpServer?.Tick();

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        MapChannelHandler.Clear();

        if (KcpServer != null)
        {
            KcpServer.Stop();
            KcpServer = null;
        }

        Logger.LogInformation("KcpServer 停止成功！");

        return Task.CompletedTask;
    }

    async void OnKcpData(int connection_id, ArraySegment<byte> data, KcpChannel kcp_channel)
    {
        //Logger.LogInformation($"OnKcpData ConnectionId={connection_id} Len={data.Count}");

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
        //Logger.LogInformation($"OnKcpConnected ConnectionId={connection_id}");

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
        //Logger.LogInformation($"OnKcpClosed ConnectionId={connection_id}");

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
        //Logger.LogInformation($"OnKcpError ConnectionId={connection_id} ErrorCode={error_code} reason={reason}");

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