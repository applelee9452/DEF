

using kcp2k;
using Microsoft.Extensions.Logging;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DEF.Gateway;

public struct SendDataItem
{
    public byte[] Data;
    public KcpPeer Peer;
}

public class KcpChannelHandler
{
    ServiceClient ServiceClient { get; set; }
    GatewayService GatewayService { get; set; }
    KcpPeer Peer { get; set; }
    EndPoint RemoteEndPoint { get; set; }
    ILogger Logger { get; set; }
    string ClientIp { get; set; }
    string PlayerGuid { get; set; }

    public void Connected(KcpPeer peer, EndPoint remove_endpoint)
    {
        ServiceClient = GatewayContext.Instance.ServiceClient;
        Logger = GatewayContext.Instance.Logger;
        Peer = peer;
        RemoteEndPoint = remove_endpoint;

        GatewayService = GatewayContext.Instance.GatewayService;

        var ep = (IPEndPoint)RemoteEndPoint;
        ClientIp = ep.Address.MapToIPv4().ToString();

        if (!GatewayContext.Instance.GatewayOptions.Value.EnableAuth)
        {
            PlayerGuid = Guid.NewGuid().ToString();
        }
    }

    public async ValueTask ClosedAsync()
    {
        Peer.Disconnect();

        if (!string.IsNullOrEmpty(PlayerGuid))
        {
            GatewayContext.Instance.MapKcpChannelHandler.TryRemove(PlayerGuid, out var chc);

            // 广播Client断开连接消息
            await ServiceClient.SessionDisConnect(PlayerGuid, string.Empty);
        }
    }

    public async Task ClosedAsync2()
    {
        if (!string.IsNullOrEmpty(PlayerGuid))
        {
            GatewayContext.Instance.MapKcpChannelHandler.TryRemove(PlayerGuid, out var chc);

            // 广播Client断开连接消息
            await ServiceClient.SessionDisConnect(PlayerGuid, string.Empty);
        }
    }

    public async ValueTask OnRecvPackage(ArraySegment<byte> packet)
    {
        RpcData rpc_data = RpcDataHelper.UnPack(packet.Array, packet.Offset, packet.Count);

        if (rpc_data.ServiceName == "def.gateway")
        {
            byte[] recv_data = await GatewayService.OnRecvPackage(rpc_data, ClosedAsync, ClientIp, string.Empty);

            if (recv_data != null)
            {
                SendData(recv_data);
            }
        }
        else if (!string.IsNullOrEmpty(PlayerGuid))
        {
            //Logger.LogInformation("前端收到Client数据 ServiceName={0}，MethodName={1}，TotalDataLen={2}",
            //    rpc_data.ServiceName, rpc_data.MethodName, rpc_data.TotalDataLen);

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

                    byte[] buff_response2 = RpcDataHelper.Pack(rpc_data, false);

                    SendData(buff_response2);
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
            await ClosedAsync();

            return;
        }
    }

    public void SendData(byte[] data)
    {
        //Peer.SendData(data, KcpChannel.Reliable);

        SendDataItem item;
        item.Data = data;
        item.Peer = Peer;

        GatewayContext.Instance.QueKcpSendData.Enqueue(item);
    }

    void SetPlayerGuid(string player_guid)
    {
        if (string.IsNullOrEmpty(player_guid)) return;

        PlayerGuid = player_guid;

        GatewayContext.Instance.MapKcpChannelHandler.TryAdd(player_guid, this);
    }
}