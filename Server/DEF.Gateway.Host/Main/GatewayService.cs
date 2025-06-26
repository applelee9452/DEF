using Microsoft.Extensions.Options;

namespace DEF.Gateway;

// 单实例对应所有前端Session
public class GatewayService
{
    IOptions<DEFOptions> DEFOptions { get; set; }
    IOptions<ServiceClientOptions> ServiceClientOptions { get; set; }
    ServiceClient ServiceClient { get; set; }
    ILogger Logger { get; set; }

    public GatewayService(ILogger<ServiceClient> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ServiceClientOptions> serviceclient_options,
        ServiceClient service_client)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceClientOptions = serviceclient_options;
        ServiceClient = service_client;
    }

    public async Task<byte[]> OnRecvPackage(RpcData rpc_data, Func<ValueTask> on_close, string client_ip, string client_ipaddress, bool pack_data = true)
    {
        if (rpc_data.ServiceName != "def.gateway")
        {
            return null;
        }

        if (rpc_data.MethodName == "Heartbeat")
        {
            // Client心跳请求
            //Logger.LogInformation("Client心跳请求，SessionGuid={GuidSession}", GuidSession);
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
                    Logger.LogError("GatewayAuth 长，认证参数失败，断开连接！");

                    // 认证参数异常，断开连接
                    if (on_close == null)
                    {
                        return null;
                    }
                    else
                    {
                        await on_close.Invoke();
                    }

                    return null;
                }

                var gateway_options = GatewayContext.Instance.GatewayOptions.Value;

                // Client请求Gateway认证，Gateway请求UCenter认证，获取AccountAppData中关联的PlayerGuid
                var container_app = ServiceClient.GetContainerRpc<UCenter.IContainerStatelessApp>();
                var response = await container_app.AppAuth(gateway_options.AppId4UCenter, client_request.obj1.acc_id, client_request.obj1.token);

                if (response == null
                    || response.Item1 != UCenter.UCenterErrorCode.NoError
                    || response.Item2 == null
                    || string.IsNullOrEmpty(response.Item2.PlayerGuid))
                {
                    Logger.LogError("Gateway转调UCenter 长，认证失败，断开连接！");

                    // 认证异常，断开连接
                    if (on_close == null)
                    {
                        return null;
                    }
                    else
                    {
                        await on_close.Invoke();
                    }

                    return null;
                }

                // 给长连接关联上PlayerGuid
                {
                    var tcpserver_type = GatewayContext.Instance.TcpServerType;

                    DotNetty.Transport.Channels.IChannelHandlerContext ch1 = null;
                    SuperSocketChannelHandler ch2 = null;
                    KcpChannelHandler ch3 = null;
                    WebSocketHandler ch4 = null;

                    if (tcpserver_type == TcpServerType.DotNetty)
                    {
                        GatewayContext.Instance.MapDotNettyChannelHandler.TryGetValue(client_request.obj1.session_id, out ch1);
                        if (ch1 == null)
                        {
                            // log error
                            return null;
                        }

                        //ch1.SetPlayerGuid(response.Item2.PlayerGuid);
                    }
                    else if (tcpserver_type == TcpServerType.SuperSocket)
                    {
                        GatewayContext.Instance.MapSuperSocketChannelHandler.TryGetValue(client_request.obj1.session_id, out ch2);
                        if (ch2 == null)
                        {
                            // log error
                            return null;
                        }

                        ch2.SetPlayerGuid(response.Item2.PlayerGuid);
                    }
                    else if (tcpserver_type == TcpServerType.Kcp)
                    {
                        GatewayContext.Instance.MapKcpChannelHandler.TryGetValue(client_request.obj1.session_id, out ch3);
                        if (ch3 == null)
                        {
                            // log error
                            return null;
                        }

                        //ch3.SetPlayerGuid(response.Item2.PlayerGuid);
                    }
                    else if (tcpserver_type == TcpServerType.WebSocket)
                    {
                        GatewayContext.Instance.MapWebSocketHandler.TryGetValue(client_request.obj1.session_id, out ch4);
                        if (ch4 == null)
                        {
                            // log error
                            return null;
                        }

                        ch4.SetPlayerGuid(response.Item2.PlayerGuid);
                    }
                }

                if (!string.IsNullOrEmpty(client_ipaddress))
                {
                    Logger.LogInformation("Gateway.Auth，{PlayerGuid}，{NickName}，{Platform}{Channel}，{ClientIp}({client_ipaddress})",
                       response.Item2.PlayerGuid,
                       client_request.obj1.nick_name,
                       client_request.obj1.platform,
                       client_request.obj1.channel_id,
                       client_ip,
                       client_ipaddress);
                }
                else
                {
                    Logger.LogInformation("Gateway.Auth，{PlayerGuid}，{NickName}，{Platform}{Channel}，{ClientIp}",
                       response.Item2.PlayerGuid,
                       client_request.obj1.nick_name,
                       client_request.obj1.platform,
                       client_request.obj1.channel_id,
                       client_ip);
                }

                // 广播Session认证成功消息

                var info = new GatewayAuthedInfo()
                {
                    AccId = client_request.obj1.acc_id,
                    SessionGuid = client_request.obj1.session_id,
                    GatewayGuid = ServiceClient.Id,
                    PlayerGuid = response.Item2.PlayerGuid,
                    ClientIp = client_ip,
                    ClientIpAddress = client_ipaddress,
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

                byte[] response_data = EntitySerializer.Serialize(def_options.SerializerType, client_auth_response);

                if (response_data != null)
                {
                    if (pack_data)
                    {
                        rpc_data.MethodData = response_data;
                        rpc_data.MethodDataLen = response_data.Length;

                        byte[] buff_response2 = RpcDataHelper.Pack(rpc_data);

                        return buff_response2;
                    }
                    else
                    {
                        return response_data;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError("认证异常，断开连接！Exception={0}", e.ToString());

                // 认证异常，断开连接
                if (on_close == null)
                {
                    return null;
                }
                else
                {
                    await on_close.Invoke();
                }

                return null;
            }
        }
        //else if (rpc_data.MethodName == "AuthNo")
        //{
        //    try
        //    {
        //        if (GatewayContext.Instance.GatewayOptions.Value.EnableAuth)
        //        {
        //            Logger.LogError("Client免认证请求，需要认证，断开连接！");

        //            // 认证异常，断开连接
        //            if (on_close == null)
        //            {
        //                return null;
        //            }
        //            else
        //            {
        //                await on_close.Invoke();
        //            }

        //            return null;
        //        }

        //        // 解析Client请求

        //        SerializeObj<ClientAuthNoRequest> client_request = null;

        //        var def_options = GatewayContext.Instance.DEFOptions.Value;
        //        client_request = EntitySerializer.Deserialize<SerializeObj<ClientAuthNoRequest>>(def_options.SerializerType, rpc_data.MethodData);

        //        if (client_request == null)
        //        {
        //            Logger.LogError("Client免认证请求，认证参数失败，断开连接！");

        //            // 认证参数异常，断开连接
        //            if (on_close == null)
        //            {
        //                return null;
        //            }
        //            else
        //            {
        //                await on_close.Invoke();
        //            }

        //            return null;
        //        }

        //        Logger.LogInformation("Client免认证请求，DeviceId={DeviceId}",
        //            client_request.obj1.device_id);

        //        on_set_playerguid?.Invoke(session_guid);

        //        // 广播Session认证成功消息

        //        var info = new GatewayAuthedInfo()
        //        {
        //            AccId = string.Empty,
        //            SessionGuid = session_guid,
        //            GatewayGuid = ServiceClient.Id,
        //            ClientIp = client_ip,
        //            ClientIpAddress = client_ipaddress,
        //            Gender = 0,
        //            NickName = string.Empty,
        //            ChannelId = string.Empty,
        //            Platform = string.Empty,
        //            Region = string.Empty,
        //            Lan = string.Empty,
        //            InviteId = string.Empty,
        //            DeviceId = string.Empty,
        //        };

        //        await ServiceClient.SessionAuthed(info, string.Empty);

        //        // 告知Client认证结果
        //        ClientAuthResponse client_auth_response = new()
        //        {
        //            result = 0,
        //            PlayerGuid = session_guid,
        //        };

        //        byte[] response_data = null;
        //        response_data = EntitySerializer.Serialize(def_options.SerializerType, client_auth_response);

        //        if (response_data != null)
        //        {
        //            if (pack_data)
        //            {
        //                rpc_data.MethodData = response_data;
        //                rpc_data.MethodDataLen = response_data.Length;

        //                byte[] buff_response2 = RpcDataHelper.Pack(rpc_data);

        //                return buff_response2;
        //            }
        //            else
        //            {
        //                return response_data;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.LogError("认证异常，断开连接！Exception={0}", e.ToString());

        //        // 认证异常，断开连接
        //        if (on_close == null)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            await on_close.Invoke();
        //        }

        //        return null;
        //    }
        //}
        else
        {
            // 异常请求
            Logger.LogError("异常请求，断开连接");

            // 异常请求，断开连接
            if (on_close == null)
            {
                return null;
            }
            else
            {
                await on_close.Invoke();
            }

            return null;
        }

        return null;
    }
}