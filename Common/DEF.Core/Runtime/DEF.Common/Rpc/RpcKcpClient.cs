#if DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DEF
{
    public class RpcKcpClientTask
    {
        public float Tm;
        public TaskCompletionSource<RpcData> Tcs;
    }

    public class RpcKcpClient
    {
        public bool IsConnected { get { return (NetworkClientKcp == null) ? false : NetworkClientKcp.IsConnected; } }

        public Action ActionSocketConnected;
        public Action ActionSocketClosed;
        public Action<KcpErrorCode, string> ActionSocketError;

        NetworkClientKcp NetworkClientKcp;
        readonly string Host;
        readonly int Port;
        readonly IClient Client;
        readonly Dictionary<ushort, RpcKcpClientTask> MapTask = new();
        readonly Queue<ushort> QueFreeTicket = new();
        ushort MaxTicket = 0;

        const float Timeout = 5000f;// 默认5秒没有返回则超时

        public RpcKcpClient(IClient client, string host, int port)
        {
            Client = client;
            Host = host;
            Port = port;

            if (NetworkClientKcp == null)
            {
                var go_def = GameObject.Find("DEF");
                if (go_def == null)
                {
                    go_def = new GameObject("DEF");
                }

                var go_networkkcpclient = new GameObject("NetworkKcpClient");
                go_networkkcpclient.transform.parent = go_def.transform;

                NetworkClientKcp = go_networkkcpclient.GetComponent<NetworkClientKcp>();
                if (NetworkClientKcp == null)
                {
                    NetworkClientKcp = go_networkkcpclient.AddComponent<NetworkClientKcp>();
                }

                NetworkClientKcp.OnRpcMethod = OnSocketBinary;
                NetworkClientKcp.OnSocketConnected = OnSocketConnected;
                NetworkClientKcp.OnSocketClosed = OnSocketClosed;
                NetworkClientKcp.OnSocketError = OnSocketError;
            }
        }

        public void Connect()
        {
            NetworkClientKcp.Connect(Host, Port);
        }

        public void Close()
        {
            NetworkClientKcp.Disconnect();
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

            if (NetworkClientKcp == null)
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

            if (NetworkClientKcp != null)
            {
                NetworkClientKcp.Send(buff);
            }

            return Task.CompletedTask;
        }

        public async Task<TResult> RequestResponse<TResult, TSerializeObj>(RpcInfoClientUnityILR rpcinfo, string method_name, TSerializeObj so) where TSerializeObj : SerializeObj
        {
            if (NetworkClientKcp == null)
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

            if (NetworkClientKcp != null)
            {
                NetworkClientKcp.Send(buff);
            }

            var t = new RpcKcpClientTask()
            {
                Tm = 0,
                Tcs = new TaskCompletionSource<RpcData>(),
            };
            MapTask[rpc_data.Ticket] = t;

            var response = await t.Tcs.Task;

            var r = EntitySerializer.Deserialize<TResult>(Client.Config.SerializerType, response.MethodData);

            return r;
        }

        void OnSocketBinary(byte[] data, int index, int len)
        {
            RpcData rpc_data = RpcDataHelper.UnPack(data, index, len);

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
            ActionSocketConnected?.Invoke();
        }

        void OnSocketClosed()
        {
            ActionSocketClosed?.Invoke();
        }

        void OnSocketError(KcpErrorCode error_code, string reason)
        {
            ActionSocketError?.Invoke(error_code, reason);
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