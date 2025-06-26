using System.Net;
using SuperSocket;
using SuperSocket.Channel;
using SuperSocket.Server;
using SuperSocket.Server.Abstractions.Session;

namespace DEF.Gateway;

public class SuperSocketChannelHandler : AppSession
{
    ServiceClient ServiceClient { get; set; }
    GatewayService GatewayService { get; set; }
    string ClientIp { get; set; }
    string PlayerGuid { get; set; }
    string SessionGuid { get; set; }
    ulong RecvFromClient { get; set; }
    ulong SendToClient { get; set; }

    public SuperSocketChannelHandler(ServiceClient service_client)
    {
        ServiceClient = service_client;

        GatewayService = GatewayContext.Instance.GatewayService;
    }

    protected override ValueTask OnSessionConnectedAsync()
    {
        var ep = (IPEndPoint)RemoteEndPoint;
        ClientIp = ep.Address.MapToIPv4().ToString();
        SessionGuid = Guid.NewGuid().ToString();

        GatewayContext.Instance.MapSuperSocketChannelHandler.TryAdd(SessionGuid, this);

        // 连接成功后给Client推送SessionGuid
        RpcData rpc_data = new()
        {
            MethodName = SessionGuid,
        };

        var data = RpcDataHelper.Pack(rpc_data, true);

        SendData2(data);

        return ValueTask.CompletedTask;
    }

    protected override async ValueTask OnSessionClosedAsync(SuperSocket.Connection.CloseEventArgs e)
    {
        if (!string.IsNullOrEmpty(SessionGuid))
        {
            GatewayContext.Instance.MapSuperSocketChannelHandler.TryRemove(SessionGuid, out var chc);

            // 广播Client断开连接消息
            if (!string.IsNullOrEmpty(PlayerGuid))
            {
                await ServiceClient.SessionDisConnect(PlayerGuid, SessionGuid);
            }
        }
    }

    public async ValueTask OnRecvPackage(SuperSocketPacketInfo packet)
    {
        RecvFromClient += (ulong)packet.Data.Length;

        RpcData rpc_data = RpcDataHelper.UnPack(packet.Data, 0, packet.Data.Length);

        if (!string.IsNullOrEmpty(PlayerGuid))
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

                    byte[] buff_response2 = RpcDataHelper.Pack(rpc_data, true);

                    SendData2(buff_response2);
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

    public ValueTask<bool> OnRecvError(PackageHandlingException<SuperSocketPacketInfo> packet)
    {
        return ValueTask.FromResult(true);
    }

    //public ValueTask SendData(byte[] data)
    //{
    //    return Channel.SendAsync(data);
    //}

    public void SendData2(byte[] data)
    {
        //return Channel.SendAsync(data);

        SendToClient += (ulong)data.Length;

        ((IAppSession)this).SendAsync(data);
    }

    public void SetPlayerGuid(string player_guid)
    {
        PlayerGuid = player_guid;
    }
}