using DotNetty.Buffers;

namespace DEF.Gateway;

public class ServiceClientObserverListener : IServiceClientObserverListener
{
    public Task NotifySession(ObserverInfo observer_info, string session_guid,
        string method_name)
    {
        return Notify(observer_info, session_guid, method_name, null);
    }

    public Task NotifySession<T1>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1)
    {
        var so = new SerializeObj<T1>()
        {
            obj1 = obj1,
        };

        var def_options = GatewayContext.Instance.DEFOptions.Value;

        byte[] method_data = null;
        if (so != null)
        {
            method_data = EntitySerializer.Serialize(def_options.SerializerType, so);
        }

        return Notify(observer_info, session_guid, method_name, method_data);
    }

    public Task NotifySession<T1, T2>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2)
    {
        var so = new SerializeObj<T1, T2>()
        {
            obj1 = obj1,
            obj2 = obj2,
        };

        var def_options = GatewayContext.Instance.DEFOptions.Value;

        byte[] method_data = null;
        if (so != null)
        {
            method_data = EntitySerializer.Serialize(def_options.SerializerType, so);
        }

        return Notify(observer_info, session_guid, method_name, method_data);
    }

    public Task NotifySession<T1, T2, T3>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3)
    {
        var so = new SerializeObj<T1, T2, T3>()
        {
            obj1 = obj1,
            obj2 = obj2,
            obj3 = obj3,
        };

        var def_options = GatewayContext.Instance.DEFOptions.Value;

        byte[] method_data = null;
        if (so != null)
        {
            method_data = EntitySerializer.Serialize(def_options.SerializerType, so);
        }

        return Notify(observer_info, session_guid, method_name, method_data);
    }

    public Task NotifySession<T1, T2, T3, T4>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4)
    {
        var so = new SerializeObj<T1, T2, T3, T4>()
        {
            obj1 = obj1,
            obj2 = obj2,
            obj3 = obj3,
            obj4 = obj4,
        };

        var def_options = GatewayContext.Instance.DEFOptions.Value;

        byte[] method_data = null;
        if (so != null)
        {
            method_data = EntitySerializer.Serialize(def_options.SerializerType, so);
        }

        return Notify(observer_info, session_guid, method_name, method_data);
    }

    public Task NotifySession<T1, T2, T3, T4, T5>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5)
    {
        var so = new SerializeObj<T1, T2, T3, T4, T5>()
        {
            obj1 = obj1,
            obj2 = obj2,
            obj3 = obj3,
            obj4 = obj4,
            obj5 = obj5,
        };

        var def_options = GatewayContext.Instance.DEFOptions.Value;

        byte[] method_data = null;
        if (so != null)
        {
            method_data = EntitySerializer.Serialize(def_options.SerializerType, so);
        }

        return Notify(observer_info, session_guid, method_name, method_data);
    }

    public Task NotifySession<T1, T2, T3, T4, T5, T6>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6)
    {
        var so = new SerializeObj<T1, T2, T3, T4, T5, T6>()
        {
            obj1 = obj1,
            obj2 = obj2,
            obj3 = obj3,
            obj4 = obj4,
            obj5 = obj5,
            obj6 = obj6,
        };

        var def_options = GatewayContext.Instance.DEFOptions.Value;

        byte[] method_data = null;
        if (so != null)
        {
            method_data = EntitySerializer.Serialize(def_options.SerializerType, so);
        }

        return Notify(observer_info, session_guid, method_name, method_data);
    }

    public Task NotifySession<T1, T2, T3, T4, T5, T6, T7>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7)
    {
        var so = new SerializeObj<T1, T2, T3, T4, T5, T6, T7>()
        {
            obj1 = obj1,
            obj2 = obj2,
            obj3 = obj3,
            obj4 = obj4,
            obj5 = obj5,
            obj6 = obj6,
            obj7 = obj7,
        };

        var def_options = GatewayContext.Instance.DEFOptions.Value;

        byte[] method_data = null;
        if (so != null)
        {
            method_data = EntitySerializer.Serialize(def_options.SerializerType, so);
        }

        return Notify(observer_info, session_guid, method_name, method_data);
    }

    public Task NotifySession<T1, T2, T3, T4, T5, T6, T7, T8>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8)
    {
        var so = new SerializeObj<T1, T2, T3, T4, T5, T6, T7, T8>()
        {
            obj1 = obj1,
            obj2 = obj2,
            obj3 = obj3,
            obj4 = obj4,
            obj5 = obj5,
            obj6 = obj6,
            obj7 = obj7,
            obj8 = obj8,
        };

        var def_options = GatewayContext.Instance.DEFOptions.Value;

        byte[] method_data = null;
        if (so != null)
        {
            method_data = EntitySerializer.Serialize(def_options.SerializerType, so);
        }

        return Notify(observer_info, session_guid, method_name, method_data);
    }

    public Task NotifySession<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9)
    {
        var so = new SerializeObj<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
        {
            obj1 = obj1,
            obj2 = obj2,
            obj3 = obj3,
            obj4 = obj4,
            obj5 = obj5,
            obj6 = obj6,
            obj7 = obj7,
            obj8 = obj8,
            obj9 = obj9,
        };

        var def_options = GatewayContext.Instance.DEFOptions.Value;

        byte[] method_data = null;
        if (so != null)
        {
            method_data = EntitySerializer.Serialize(def_options.SerializerType, so);
        }

        return Notify(observer_info, session_guid, method_name, method_data);
    }

    public async Task DisConnectSession(string session_guid, string reason)
    {
        var tcpserver_type = GatewayContext.Instance.TcpServerType;

        if (tcpserver_type == TcpServerType.DotNetty)
        {
            var m = GatewayContext.Instance.MapDotNettyChannelHandler;
            m.Remove(session_guid, out var ch);
            if (ch != null)
            {
                await ch.CloseAsync();
            }
        }
        else if (tcpserver_type == TcpServerType.SuperSocket)
        {
            var m = GatewayContext.Instance.MapSuperSocketChannelHandler;
            m.Remove(session_guid, out var ch);
            if (ch != null)
            {
                await ch.CloseAsync();
            }
        }
        else if (tcpserver_type == TcpServerType.Kcp)
        {
            var m = GatewayContext.Instance.MapKcpChannelHandler;
            m.Remove(session_guid, out var ch);
            if (ch != null)
            {
                await ch.ClosedAsync();
            }
        }
        else if (tcpserver_type == TcpServerType.WebSocket)
        {
            var m = GatewayContext.Instance.MapWebSocketHandler;
            m.Remove(session_guid, out var ch);
            if (ch != null)
            {
                await ch.CloseAsync();
            }
        }
    }

    async Task Notify(ObserverInfo observer_info, string session_guid, string method_name, byte[] method_data)
    {
        var tcpserver_type = GatewayContext.Instance.TcpServerType;

        DotNetty.Transport.Channels.IChannelHandlerContext ch1 = null;
        SuperSocketChannelHandler ch2 = null;
        KcpChannelHandler ch3 = null;
        WebSocketHandler ch4 = null;

        if (tcpserver_type == TcpServerType.DotNetty)
        {
            GatewayContext.Instance.MapDotNettyChannelHandler.TryGetValue(session_guid, out ch1);
            if (ch1 == null)
            {
                // log error
                return;
            }
        }
        else if (tcpserver_type == TcpServerType.SuperSocket)
        {
            GatewayContext.Instance.MapSuperSocketChannelHandler.TryGetValue(session_guid, out ch2);
            if (ch2 == null)
            {
                // log error
                return;
            }
        }
        else if (tcpserver_type == TcpServerType.Kcp)
        {
            GatewayContext.Instance.MapKcpChannelHandler.TryGetValue(session_guid, out ch3);
            if (ch3 == null)
            {
                // log error
                return;
            }
        }
        else if (tcpserver_type == TcpServerType.WebSocket)
        {
            GatewayContext.Instance.MapWebSocketHandler.TryGetValue(session_guid, out ch4);
            if (ch4 == null)
            {
                // log error
                return;
            }
        }

        RpcData rpc_data = new()
        {
            Ticket = 0,
            ContainerStateType = observer_info.ContainerStateType,
            ContainerType = observer_info.ContainerType,
            ContainerId = observer_info.ContainerId,
            EntityId = observer_info.EntityId,
            ComponentName = observer_info.ComponentName,
            MethodName = method_name,
            MethodData = method_data,
            MethodDataLen = method_data == null ? 0 : method_data.Length
        };

        try
        {
            if (tcpserver_type == TcpServerType.DotNetty)
            {
                byte[] buff_response2 = RpcDataHelper.Pack(rpc_data);

                IByteBuffer buff_response = ch1.Allocator.Buffer(buff_response2.Length, buff_response2.Length);
                buff_response.WriteBytes(buff_response2);

                try
                {
                    await ch1.WriteAndFlushAsync(buff_response);
                }
                catch (Exception)
                {
                }
            }
            else if (tcpserver_type == TcpServerType.SuperSocket)
            {
                byte[] buff_response2 = RpcDataHelper.Pack(rpc_data, true);

                //await ch2.SendData(buff_response2);

                ch2.SendData2(buff_response2);
            }
            else if (tcpserver_type == TcpServerType.Kcp)
            {
                byte[] buff_response2 = RpcDataHelper.Pack(rpc_data, false);

                ch3.SendData(buff_response2);
            }
            else if (tcpserver_type == TcpServerType.WebSocket)
            {
                byte[] buff_response2 = RpcDataHelper.Pack(rpc_data);

                await ch4.SendData(buff_response2);
            }
        }
        catch (Exception ex)
        {
            GatewayContext.Instance.Logger.LogError(ex, "ServiceClientObserverListener.Notify()");
        }
    }
}