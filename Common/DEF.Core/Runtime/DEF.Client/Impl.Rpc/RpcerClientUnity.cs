#if DEF_CLIENT

using System.Threading.Tasks;

namespace DEF
{
    public class RpcerClientUnity : IRpcer
    {
        readonly ClientUnity Client;

        public RpcerClientUnity(ClientUnity client)
        {
            Client = client;
        }

        public Task RequestResponse(IRpcInfo rpcinfo,
            string method_name)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            return Request<SerializeObj>(rpcinfo2, method_name, null);
        }

        public Task RequestResponse<T1>(IRpcInfo rpcinfo,
            string method_name, T1 obj1)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1>()
            {
                obj1 = obj1,
            };

            return Request(rpcinfo2, method_name, so);
        }

        public Task RequestResponse<T1, T2>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1, T2>()
            {
                obj1 = obj1,
                obj2 = obj2,
            };

            return Request(rpcinfo2, method_name, so);
        }

        public Task RequestResponse<T1, T2, T3>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1, T2, T3>()
            {
                obj1 = obj1,
                obj2 = obj2,
                obj3 = obj3,
            };

            return Request(rpcinfo2, method_name, so);
        }

        public Task RequestResponse<T1, T2, T3, T4>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1, T2, T3, T4>()
            {
                obj1 = obj1,
                obj2 = obj2,
                obj3 = obj3,
                obj4 = obj4,
            };

            return Request(rpcinfo2, method_name, so);
        }

        public Task RequestResponse<T1, T2, T3, T4, T5>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1, T2, T3, T4, T5>()
            {
                obj1 = obj1,
                obj2 = obj2,
                obj3 = obj3,
                obj4 = obj4,
                obj5 = obj5,
            };

            return Request(rpcinfo2, method_name, so);
        }

        public Task RequestResponse<T1, T2, T3, T4, T5, T6>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1, T2, T3, T4, T5, T6>()
            {
                obj1 = obj1,
                obj2 = obj2,
                obj3 = obj3,
                obj4 = obj4,
                obj5 = obj5,
                obj6 = obj6,
            };

            return Request(rpcinfo2, method_name, so);
        }

        public Task RequestResponse<T1, T2, T3, T4, T5, T6, T7>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

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

            return Request(rpcinfo2, method_name, so);
        }

        public Task RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

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

            return Request(rpcinfo2, method_name, so);
        }

        public Task RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

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

            return Request(rpcinfo2, method_name, so);
        }

        public Task<TResult> RequestResponse<TResult>(IRpcInfo rpcinfo,
            string method_name)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            return RequestResponse<TResult, SerializeObj>(rpcinfo2, method_name, null);
        }

        public Task<TResult> RequestResponse<T1, TResult>(IRpcInfo rpcinfo,
            string method_name, T1 obj1)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1>()
            {
                obj1 = obj1,
            };

            return RequestResponse<TResult, SerializeObj<T1>>(rpcinfo2, method_name, so);
        }

        public Task<TResult> RequestResponse<T1, T2, TResult>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1, T2>()
            {
                obj1 = obj1,
                obj2 = obj2,
            };

            return RequestResponse<TResult, SerializeObj<T1, T2>>(rpcinfo2, method_name, so);
        }

        public Task<TResult> RequestResponse<T1, T2, T3, TResult>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1, T2, T3>()
            {
                obj1 = obj1,
                obj2 = obj2,
                obj3 = obj3,
            };

            return RequestResponse<TResult, SerializeObj<T1, T2, T3>>(rpcinfo2, method_name, so);
        }

        public Task<TResult> RequestResponse<T1, T2, T3, T4, TResult>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1, T2, T3, T4>()
            {
                obj1 = obj1,
                obj2 = obj2,
                obj3 = obj3,
                obj4 = obj4,
            };

            return RequestResponse<TResult, SerializeObj<T1, T2, T3, T4>>(rpcinfo2, method_name, so);
        }

        public Task<TResult> RequestResponse<T1, T2, T3, T4, T5, TResult>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1, T2, T3, T4, T5>()
            {
                obj1 = obj1,
                obj2 = obj2,
                obj3 = obj3,
                obj4 = obj4,
                obj5 = obj5,
            };

            return RequestResponse<TResult, SerializeObj<T1, T2, T3, T4, T5>>(rpcinfo2, method_name, so);
        }

        public Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, TResult>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

            var so = new SerializeObj<T1, T2, T3, T4, T5, T6>()
            {
                obj1 = obj1,
                obj2 = obj2,
                obj3 = obj3,
                obj4 = obj4,
                obj5 = obj5,
                obj6 = obj6,
            };

            return RequestResponse<TResult, SerializeObj<T1, T2, T3, T4, T5, T6>>(rpcinfo2, method_name, so);
        }

        public Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, T7, TResult>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

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

            return RequestResponse<TResult, SerializeObj<T1, T2, T3, T4, T5, T6, T7>>(rpcinfo2, method_name, so);
        }

        public Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

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

            return RequestResponse<TResult, SerializeObj<T1, T2, T3, T4, T5, T6, T7, T8>>(rpcinfo2, method_name, so);
        }

        public Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(IRpcInfo rpcinfo,
            string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9)
        {
            var rpcinfo2 = (RpcInfoClientUnityILR)rpcinfo;

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

            return RequestResponse<TResult, SerializeObj<T1, T2, T3, T4, T5, T6, T7, T8, T9>>(rpcinfo2, method_name, so);
        }

        public Task Request<TSerializeObj>(RpcInfoClientUnityILR rpcinfo, string method_name, TSerializeObj so) where TSerializeObj : SerializeObj
        {
            var rpc_client = Client.GetTargetServiceRpcClient(rpcinfo.TargetServiceName.ToLower());

            //Debug.Log($"GetServerRpcClient TargetServiceName={rpcinfo.TargetServiceName.ToLower()}, RpcClient={rpc_client}");

            if (rpc_client == "HttpClient")
            {
                return Client.RpcHttpClient.Request<TSerializeObj>(rpcinfo, method_name, so);
            }
            else if (rpc_client == "TcpClient")
            {
                return Client.RpcTcpClient.Request<TSerializeObj>(rpcinfo, method_name, so);
            }
            else if (rpc_client == "SuperSocketClient")
            {
                return Client.RpcSuperSocketClient.Request<TSerializeObj>(rpcinfo, method_name, so);
            }
            else if (rpc_client == "NetlyClient")
            {
                return Client.RpcNetlyClient.Request<TSerializeObj>(rpcinfo, method_name, so);
            }
            else if (rpc_client == "KcpClient")
            {
                return Client.RpcKcpClient.Request<TSerializeObj>(rpcinfo, method_name, so);
            }
            else if (rpc_client == "WebSocketClient")
            {
                return Client.RpcWebSocketClient.Request<TSerializeObj>(rpcinfo, method_name, so);
            }
            else
            {
                // log error
                return Task.CompletedTask;
            }
        }

        public Task<TResult> RequestResponse<TResult, TSerializeObj>(RpcInfoClientUnityILR rpcinfo, string method_name, TSerializeObj so) where TSerializeObj : SerializeObj
        {
            var rpc_client = Client.GetTargetServiceRpcClient(rpcinfo.TargetServiceName.ToLower());

            if (rpc_client == "HttpClient")
            {
                return Client.RpcHttpClient.RequestResponse<TResult, TSerializeObj>(rpcinfo, method_name, so);
            }
            else if (rpc_client == "TcpClient")
            {
                return Client.RpcTcpClient.RequestResponse<TResult, TSerializeObj>(rpcinfo, method_name, so);
            }
            else if (rpc_client == "SuperSocketClient")
            {
                return Client.RpcSuperSocketClient.RequestResponse<TResult, TSerializeObj>(rpcinfo, method_name, so);
            }
            else if (rpc_client == "NetlyClient")
            {
                return Client.RpcNetlyClient.RequestResponse<TResult, TSerializeObj>(rpcinfo, method_name, so);
            }
            else if (rpc_client == "KcpClient")
            {
                return Client.RpcKcpClient.RequestResponse<TResult, TSerializeObj>(rpcinfo, method_name, so);
            }
            else if (rpc_client == "WebSocketClient")
            {
                return Client.RpcWebSocketClient.RequestResponse<TResult, TSerializeObj>(rpcinfo, method_name, so);
            }
            else
            {
                // log error
                return Task.FromResult(default(TResult));
            }
        }
    }
}

#endif