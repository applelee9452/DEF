#if DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DEF
{
    public class RpcNetlyClient
    {
        public Action<string> ActionSocketSessioned;
        public Action ActionSocketConnected;
        public Action ActionSocketConnectFailed;
        public Action ActionSocketClosed;
        public Action<SocketErrorEventArgs> ActionSocketError;

        NetworkClientNetly MbTcpClient;
        readonly string Host;
        readonly int Port;
        readonly IClient Client;
        readonly Dictionary<ushort, RpcTcpClientTask> MapTask = new();
        readonly Queue<ushort> QueFreeTicket = new();
        ushort MaxTicket = 0;

        const float Timeout = 5000f;// 默认5秒没有返回则超时

        public RpcNetlyClient(IClient client, string host, int port)
        {
            Client = client;
            Host = host;
            Port = port;

            if (MbTcpClient == null)
            {
                var go_def = GameObject.Find("DEF");
                if (go_def == null)
                {
                    go_def = new GameObject("DEF");
                }

                var go_networktcpclient = new GameObject("NetworkNetlyClient");
                go_networktcpclient.transform.parent = go_def.transform;

                MbTcpClient = go_networktcpclient.GetComponent<NetworkClientNetly>();
                if (MbTcpClient == null)
                {
                    MbTcpClient = go_networktcpclient.AddComponent<NetworkClientNetly>();
                }

                MbTcpClient.OnRpcMethod = OnSocketBinary;
                MbTcpClient.OnSocketConnected = OnSocketConnected;
                MbTcpClient.OnSocketConnectFailed = OnSocketConnectFailed;
                MbTcpClient.OnSocketClosed = OnSocketClosed;
                MbTcpClient.OnSocketError = OnSocketError;
            }
        }

        public void Connect()
        {
            MbTcpClient.Connect(Host, Port);
        }

        public async void Close()
        {
            await MbTcpClient.Disconnect();
        }

        public void Update(float tm)
        {
            // 超时处理

            List<ushort> list_timeout = null;

            foreach (var i in MapTask)
            {
                i.Value.Tm += tm;

                if (i.Value.Tm > Timeout)
                {
                    if (list_timeout == null)
                    {
                        list_timeout = new List<ushort>();
                    }

                    list_timeout.Add(i.Key);
                }
            }

            if (list_timeout != null)
            {
                foreach (var i in list_timeout)
                {
                    FreeTicket(i);

                    MapTask[i].Tcs.SetException(new Exception("Timeout"));
                    MapTask.Remove(i);
                }
            }
        }

        public Task Request<TSerializeObj>(RpcInfoClientUnityILR rpcinfo, string method_name, TSerializeObj so) where TSerializeObj : SerializeObj
        {
            //Debug.Log($"IsUnity={rpcinfo.IsUnity}");
            //Debug.Log($"SourceServiceName={rpcinfo.SourceServiceName}");
            //Debug.Log($"TargetServiceName={rpcinfo.TargetServiceName}");
            //Debug.Log($"ContainerOrEntity={rpcinfo.ContainerOrEntity}");
            //Debug.Log($"ContainerStateType={rpcinfo.ContainerStateType}");
            //Debug.Log($"ContainerType={rpcinfo.ContainerType}");
            //Debug.Log($"ContainerId={rpcinfo.ContainerId}");
            //Debug.Log($"EntityId={rpcinfo.EntityId}");
            //Debug.Log($"ComponentName={rpcinfo.ComponentName}");

            if (MbTcpClient == null)
            {
                Debug.LogError("Request前先建立Tcp连接！");
                return Task.CompletedTask;
            }

            RpcData rpc_data = new()
            {
                Ticket = 0,// 无需返回值
                HasResult = false,
                ServiceName = rpcinfo.TargetServiceName,
                ContainerStateType = rpcinfo.ContainerStateType,
                ContainerType = rpcinfo.ContainerType,
                ContainerId = rpcinfo.ContainerId,
                EntityId = rpcinfo.EntityId,
                ComponentName = rpcinfo.ComponentName,
                MethodName = method_name,
                TotalDataLen = 0,// 无需设置
            };

            byte[] method_data = null;
            if (so != null)
            {
                method_data = EntitySerializer.Serialize(Client.Config.SerializerType, so);
            }

            rpc_data.MethodData = method_data;
            rpc_data.MethodDataLen = method_data == null ? 0 : method_data.Length;

            byte[] buff = RpcDataHelper.Pack(rpc_data);

            if (MbTcpClient != null)
            {
                MbTcpClient.Send(buff);
            }

            return Task.CompletedTask;
        }

        public async Task<TResult> RequestResponse<TResult, TSerializeObj>(RpcInfoClientUnityILR rpcinfo, string method_name, TSerializeObj so) where TSerializeObj : SerializeObj
        {
            if (MbTcpClient == null)
            {
                Debug.LogError("Request前先建立Tcp连接！");
                return default;
            }

            RpcData rpc_data = new()
            {
                Ticket = GenTicket(),
                HasResult = true,
                ServiceName = rpcinfo.TargetServiceName,
                ContainerStateType = rpcinfo.ContainerStateType,
                ContainerType = rpcinfo.ContainerType,
                ContainerId = rpcinfo.ContainerId,
                EntityId = rpcinfo.EntityId,
                ComponentName = rpcinfo.ComponentName,
                MethodName = method_name,
                TotalDataLen = 0,// 无需设置
            };

            byte[] method_data = null;
            if (so != null)
            {
                method_data = EntitySerializer.Serialize(Client.Config.SerializerType, so);
            }

            rpc_data.MethodData = method_data;
            rpc_data.MethodDataLen = method_data == null ? 0 : method_data.Length;

            byte[] buff = RpcDataHelper.Pack(rpc_data);

            if (MbTcpClient != null)
            {
                MbTcpClient.Send(buff);
            }

            var t = new RpcTcpClientTask()
            {
                Tm = 0,
                Tcs = new TaskCompletionSource<RpcData>(),
            };
            MapTask[rpc_data.Ticket] = t;

            var response = await t.Tcs.Task;

            var r = EntitySerializer.Deserialize<TResult>(Client.Config.SerializerType, response.MethodData);

            return r;
        }

        void OnSocketBinary(byte[] data)
        {
            RpcData rpc_data = RpcDataHelper.UnPack(data, 0, data.Length);

            if (string.IsNullOrEmpty(rpc_data.ContainerType))
            {
                ActionSocketSessioned?.Invoke(rpc_data.MethodName);

                return;
            }

            if (rpc_data.Ticket == 0)
            {
                // Ticket=0表示Service主动推送的消息，Observer，进一步分发

                if (rpc_data.EntityId == 0)
                {
                    Client.DispatchContainerObserverRpc(rpc_data.ContainerType, rpc_data.MethodName, rpc_data.MethodData);
                }
                else
                {
                    Client.DispatchEntityObserverRpc(rpc_data.ContainerType, rpc_data.ContainerId, rpc_data.EntityId, rpc_data.ComponentName, rpc_data.MethodName, rpc_data.MethodData);
                }
            }
            else
            {
                MapTask.TryGetValue(rpc_data.Ticket, out var t);
                if (t != null)
                {
                    MapTask.Remove(rpc_data.Ticket);
                    FreeTicket(rpc_data.Ticket);

                    t.Tcs.SetResult(rpc_data);
                }
            }
        }

        void OnSocketConnected()
        {
            //Debug.Log("RpcTcpClient.OnSocketConnected()");

            ActionSocketConnected?.Invoke();
        }

        void OnSocketConnectFailed()
        {
            //Debug.Log("RpcTcpClient.OnSocketConnectFailed()");

            ActionSocketConnectFailed?.Invoke();
        }

        void OnSocketClosed()
        {
            //Debug.Log("RpcTcpClient.OnSocketClosed()");

            ActionSocketClosed?.Invoke();
        }

        void OnSocketError(SocketErrorEventArgs args)
        {
            //Debug.LogError("RpcTcpClient.OnSocketError()");

            ActionSocketError?.Invoke(args);
        }

        ushort GenTicket()
        {
            if (QueFreeTicket.Count > 0)
            {
                return QueFreeTicket.Dequeue();
            }
            else
            {
                ushort ticket = ++MaxTicket;
                if (ticket == 0)
                {
                    ticket = ++MaxTicket;
                }
                return ticket;
            }
        }

        void FreeTicket(ushort ticket)
        {
            QueFreeTicket.Enqueue(ticket);
        }
    }
}

#endif