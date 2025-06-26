using System.Net;
using System.Net.WebSockets;

namespace DEF.Gateway;

public class WebSocketHandler
{
    public byte[] RecvBuffer = new byte[1024 * 16];// 接受数据，上限16kb

    WebSocket WebSocket { get; set; }
    ILogger Logger { get; set; }
    ServiceClient ServiceClient { get; set; }
    GatewayService GatewayService { get; set; }
    string ClientIp { get; set; }
    string PlayerGuid { get; set; }
    string SessionGuid { get; set; }

    public WebSocketHandler(ILogger logger, WebSocket ws, ServiceClient service_client, IPAddress ip_address, string session_id)
    {
        Logger = logger;
        WebSocket = ws;
        ServiceClient = service_client;
        SessionGuid = session_id;

        GatewayContext.Instance.MapWebSocketHandler.TryAdd(SessionGuid, this);

        GatewayService = GatewayContext.Instance.GatewayService;

        if (!GatewayContext.Instance.GatewayOptions.Value.EnableAuth)
        {
            PlayerGuid = Guid.NewGuid().ToString();
        }

        ClientIp = ip_address.MapToIPv4().ToString();
    }

    public ValueTask SendSessionId()
    {
        RpcData rpc_data = new()
        {
            Ticket = 0,
            ContainerType = string.Empty,
            ContainerId = string.Empty,
            MethodName = SessionGuid,
        };

        var data = RpcDataHelper.Pack(rpc_data, false);

        return SendData(data);
    }

    public ValueTask SendData(byte[] data)
    {
        return WebSocket.SendAsync(new ReadOnlyMemory<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None);
    }

    public async ValueTask CloseAsync()
    {
        if (!string.IsNullOrEmpty(SessionGuid))
        {
            GatewayContext.Instance.MapWebSocketHandler.TryRemove(SessionGuid, out var chc);

            // 广播Client断开连接消息
            if (!string.IsNullOrEmpty(PlayerGuid))
            {
                await ServiceClient.SessionDisConnect(PlayerGuid, SessionGuid);
            }

            SessionGuid = string.Empty;
            PlayerGuid = string.Empty;
        }

        try
        {
            if (!WebSocket.CloseStatus.HasValue)
            {
                await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message);
        }
    }

    public async Task OnRecvPackage(byte[] packet, int len)
    {
        RpcData rpc_data = RpcDataHelper.UnPack(packet, 0, len);

        if (rpc_data.ServiceName == "def.gateway")
        {
            byte[] recv_data = await GatewayService.OnRecvPackage(rpc_data, CloseAsync, ClientIp, string.Empty);

            if (recv_data != null)
            {
                await SendData(recv_data);
            }
        }
        else if (!string.IsNullOrEmpty(PlayerGuid))
        {
            Logger.LogInformation("前端收到Client数据 ServiceName={ServiceName}，MethodName={MethodName}，TotalDataLen={TotalDataLen}",
                rpc_data.ServiceName, rpc_data.MethodName, rpc_data.TotalDataLen);

            // Tcp连接上的Request或RequestResponse消息，转发给Backend对应的Service

            if (rpc_data.HasResult)
            {
                byte[] response_data = null;

                if (rpc_data.EntityId == 0)
                {
                    response_data = await ServiceClient.ForwardContainerRpc(
                       rpc_data.ServiceName, (int)rpc_data.ContainerStateType,
                       rpc_data.ContainerType, rpc_data.ContainerId,
                       rpc_data.MethodName, rpc_data.MethodData);
                }
                else
                {
                    response_data = await ServiceClient.ForwardEntityRpc(
                       rpc_data.ServiceName,
                       (int)rpc_data.ContainerStateType,
                       rpc_data.ContainerType, rpc_data.ContainerId,
                       rpc_data.EntityId, rpc_data.ComponentName,
                       rpc_data.MethodName, rpc_data.MethodData);
                }

                // 返回给Client，除了MethodData不一样，其余Route信息完全一样
                if (response_data != null)
                {
                    rpc_data.MethodData = response_data;
                    rpc_data.MethodDataLen = response_data.Length;

                    byte[] buff_response2 = RpcDataHelper.Pack(rpc_data);

                    await SendData(buff_response2);
                }
            }
            else
            {
                if (rpc_data.EntityId == 0)
                {
                    await ServiceClient.ForwardContainerRpcNoResult(
                        rpc_data.ServiceName, (int)rpc_data.ContainerStateType,
                        rpc_data.ContainerType, rpc_data.ContainerId,
                        rpc_data.MethodName, rpc_data.MethodData);
                }
                else
                {
                    await ServiceClient.ForwardEntityRpcNoResult(
                        rpc_data.ServiceName,
                        (int)rpc_data.ContainerStateType,
                        rpc_data.ContainerType, rpc_data.ContainerId,
                        rpc_data.EntityId, rpc_data.ComponentName,
                        rpc_data.MethodName, rpc_data.MethodData);
                }
            }
        }
        else
        {
            // 异常请求，断开连接
            await CloseAsync();

            return;
        }
    }

    public void SetPlayerGuid(string player_guid)
    {
        if (string.IsNullOrEmpty(player_guid)) return;

        PlayerGuid = player_guid;
    }
}