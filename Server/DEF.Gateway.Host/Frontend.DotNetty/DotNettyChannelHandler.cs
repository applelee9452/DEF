

using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DEF.Gateway;

public class DotNettyChannelHandler : ChannelHandlerAdapter
{
    IChannelHandlerContext ChannelHandlerContext;
    readonly Guid GuidChannel = Guid.NewGuid();
    string PlayerGuid = string.Empty;
    string ClientIp = string.Empty;
    ILogger Logger;
    ServiceClient ServiceClient;

    public override void ChannelActive(IChannelHandlerContext context)
    {
        base.ChannelActive(context);

        ChannelHandlerContext = context;
        Logger = GatewayContext.Instance.Logger;
        ServiceClient = GatewayContext.Instance.ServiceClient;

        var ep = (IPEndPoint)context.Channel.RemoteAddress;
        ClientIp = ep.Address.MapToIPv4().ToString();

        Logger.LogInformation("前端建立Client连接 ClientGuid={GuidChannel}，ClientIp={ClientIp}", GuidChannel.ToString(), ClientIp);
    }

    public override async void ChannelInactive(IChannelHandlerContext context)
    {
        base.ChannelInactive(context);

        Logger.LogInformation("前端断开Client连接 ClientGuid={ClientGuid}，ClientIp={ClientIp}", GuidChannel.ToString(), ClientIp);

        if (!string.IsNullOrEmpty(PlayerGuid))
        {
            GatewayContext.Instance.MapDotNettyChannelHandler.TryRemove(PlayerGuid, out var _);

            // 广播Client断开连接消息
            await ServiceClient.SessionDisConnect(PlayerGuid, string.Empty);
        }
    }

    public override async void ChannelRead(IChannelHandlerContext context, object message)
    {
        var byte_buf = message as IByteBuffer;

        RpcData rpc_data = RpcDataHelper.UnPack(byte_buf);

        context.FireChannelRead(message);
        //ReferenceCountUtil.Release(message);

        if (rpc_data.ServiceName == "def.gateway")
        {
            if (rpc_data.MethodName == "Heartbeat")
            {
                // Client心跳请求
                Logger.LogInformation("Client心跳请求，PlayerGuid={PlayerGuid}", PlayerGuid);
            }
            else if (rpc_data.MethodName == "Auth")
            {
                try
                {
                    // 解析Client请求

                    SerializeObj<ClientAuthRequest> client_request = null;

                    var def_options = GatewayContext.Instance.DEFOptions.Value;
                    client_request = EntitySerializer.Deserialize<SerializeObj<ClientAuthRequest>>(def_options.SerializerType, rpc_data.MethodData);

                    if (client_request == null)
                    {
                        Logger.LogError("GatewayAuth，认证参数失败，断开连接！");

                        // 认证参数异常，断开连接
                        await context.CloseAsync();

                        return;
                    }

                    Logger.LogInformation("Client认证请求，AccId={AccId} NickName={NickName} Platform={Platform}",
                        client_request.obj1.acc_id,
                        client_request.obj1.nick_name,
                        client_request.obj1.platform);

                    var gateway_options = GatewayContext.Instance.GatewayOptions.Value;

                    // Client请求Gateway认证，Gateway请求UCenter认证
                    var container_app = ServiceClient.GetContainerRpc<UCenter.IContainerStatelessApp>();
                    var response = await container_app.AppAuth(gateway_options.AppId4UCenter, client_request.obj1.acc_id, client_request.obj1.token);

                    if (response == null
                        || response.Item1 != UCenter.UCenterErrorCode.NoError
                        || response.Item2 == null
                        || string.IsNullOrEmpty(response.Item2.PlayerGuid))
                    {
                        Logger.LogError("Gateway转调UCenter 短，认证失败，断开连接！");

                        // 认证异常，断开连接
                        await context.CloseAsync();

                        return;
                    }

                    PlayerGuid = response.Item2.PlayerGuid;

                    GatewayContext.Instance.MapDotNettyChannelHandler.TryAdd(PlayerGuid, context);

                    // 广播Session认证成功消息

                    var info = new GatewayAuthedInfo()
                    {
                        AccId = client_request.obj1.acc_id,
                        SessionGuid = PlayerGuid,
                        GatewayGuid = ServiceClient.Id,
                        ClientIp = ClientIp,
                        ClientIpAddress = string.Empty,
                        Gender = response.Item2.Gender,
                        NickName = client_request.obj1.nick_name,
                        ChannelId = client_request.obj1.channel_id,
                        Platform = client_request.obj1.platform,
                        Region = client_request.obj1.region,
                        Lan = client_request.obj1.lan,
                        InviteId = client_request.obj1.invite_id,
                        DeviceId = client_request.obj1.device_id,
                    };

                    await ServiceClient.SessionAuthed(info, string.Empty);

                    // 告知Client认证结果
                    ClientAuthResponse client_auth_response = new()
                    {
                        result = 0,
                        PlayerGuid = response.Item2.PlayerGuid,
                    };

                    byte[] response_data = null;
                    response_data = EntitySerializer.Serialize(def_options.SerializerType, client_auth_response);

                    if (response_data != null)
                    {
                        rpc_data.MethodData = response_data;
                        rpc_data.MethodDataLen = response_data.Length;

                        byte[] buff_response2 = RpcDataHelper.Pack(rpc_data);

                        IByteBuffer buff_response = ChannelHandlerContext.Allocator.Buffer(buff_response2.Length, buff_response2.Length);
                        buff_response.WriteBytes(buff_response2);

                        try
                        {
                            await ChannelHandlerContext.WriteAndFlushAsync(buff_response);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError("Auth异常，断开连接！Exception={e}", e.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError("认证异常，断开连接！Exception={e}", e.ToString());

                    // 认证异常，断开连接
                    await context.CloseAsync();

                    return;
                }
            }
            else
            {
                // 异常请求
                Logger.LogError("异常请求，断开连接");

                // 异常请求，断开连接
                await context.CloseAsync();

                return;
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

                    IByteBuffer buff_response = ChannelHandlerContext.Allocator.Buffer(buff_response2.Length, buff_response2.Length);
                    buff_response.WriteBytes(buff_response2);

                    try
                    {
                        await ChannelHandlerContext.WriteAndFlushAsync(buff_response);
                    }
                    catch (Exception)
                    {
                    }
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
            await context.CloseAsync();

            return;
        }
    }

    public override void ChannelReadComplete(IChannelHandlerContext context)
    {
        context.Flush();
    }

    public override void ExceptionCaught(IChannelHandlerContext context, Exception ex)
    {
        if (ex is ObjectDisposedException)
        {
            // do nothting
        }

        Console.WriteLine(ex.ToString());

        context.CloseAsync();
    }
}