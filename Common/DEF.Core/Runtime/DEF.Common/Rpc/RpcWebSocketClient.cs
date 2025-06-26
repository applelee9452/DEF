#if DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityWebSocket;

namespace DEF
{
    public class RpcWebSocketClientTask
    {
        public float Tm;
        public TaskCompletionSource<RpcData> Tcs;
    }

    public class RpcWebSocketClient
    {
        public Action ActionSocketConnected;
        public Action<string> ActionSocketSessioned;
        public Action ActionSocketClosed;
        public Action ActionSocketError;

        string Url { get; set; }
        IClient Client { get; set; }
        WebSocket WebSocket { get; set; }
        Dictionary<ushort, RpcTcpClientTask> MapTask { get; set; } = new();
        Queue<ushort> QueFreeTicket { get; set; } = new();
        ushort MaxTicket { get; set; } = 0;

        const float Timeout = 500f;// 默认5秒没有返回则超时

        //public string url = "ws://localhost:8080/ws";

        public RpcWebSocketClient(IClient client, string url)
        {
            Client = client;
            Url = url;
        }

        public void Connect()
        {
            WebSocket = new WebSocket(Url);
            WebSocket.OnOpen += OnWebSocketOpen;
            WebSocket.OnClose += OnWebSocketClosed;
            WebSocket.OnMessage += OnWebSocketMessage;
            WebSocket.OnError += OnWebSocketError;

            WebSocket.ConnectAsync();
        }


        public void Close()
        {
            if (WebSocket != null)
            {
                WebSocket.CloseAsync();
                WebSocket = null;
            }
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
                    list_timeout ??= new List<ushort>();

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

            if (WebSocket == null)
            {
                Debug.LogError("Request前先建立WebSocket连接！");
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

            WebSocket?.SendAsync(buff);

            return Task.CompletedTask;
        }

        public async Task<TResult> RequestResponse<TResult, TSerializeObj>(RpcInfoClientUnityILR rpcinfo, string method_name, TSerializeObj so) where TSerializeObj : SerializeObj
        {
            if (WebSocket == null)
            {
                Debug.LogError("Request前先建立WebSocket连接！");
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

            WebSocket?.SendAsync(buff);

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

        void OnWebSocketOpen(object sender, OpenEventArgs e)
        {
            Debug.Log("RpcWebSocketClient.OnWebSocketOpen()");

            ActionSocketConnected?.Invoke();
        }

        private void OnWebSocketMessage(object sender, MessageEventArgs bs)
        {
            //if (bs.IsBinary)
            //{
            //    Debug.Log(string.Format("Receive Bytes ({1}): {0}", bs.Data, bs.RawData.Length));
            //}
            //else if (bs.IsText)
            //{
            //    Debug.Log(string.Format("Receive: {0}", bs.Data));
            //}

            RpcData rpc_data = RpcDataHelper.UnPack(bs.RawData, 0, bs.RawData.Length);

            if (rpc_data.Ticket == 0 && string.IsNullOrEmpty(rpc_data.ContainerType))
            {
                ActionSocketSessioned?.Invoke(rpc_data.MethodName);

                return;
            }

            if (rpc_data.Ticket == 0)
            {
                //Debug.Log($"WebSocket.OnWebSocketBinaryNoAlloc 推送消息~~~~~~");

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
                //Debug.Log($"WebSocket.OnWebSocketBinaryNoAlloc 响应消息");

                MapTask.TryGetValue(rpc_data.Ticket, out var t);
                if (t != null)
                {
                    MapTask.Remove(rpc_data.Ticket);
                    FreeTicket(rpc_data.Ticket);

                    t.Tcs.SetResult(rpc_data);
                }
            }
        }

        /*void OnWebSocketBinaryNoAlloc(WebSocket webSocket, BufferSegment bs)
        {
            RpcData rpc_data = RpcDataHelper.UnPack(bs.Data, bs.Offset, bs.Count);

            if (rpc_data.Ticket == 0 && string.IsNullOrEmpty(rpc_data.ContainerType))
            {
                ActionSocketSessioned?.Invoke(rpc_data.MethodName);

                return;
            }

            if (rpc_data.Ticket == 0)
            {
                //Debug.Log($"WebSocket.OnWebSocketBinaryNoAlloc 推送消息~~~~~~");

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
                //Debug.Log($"WebSocket.OnWebSocketBinaryNoAlloc 响应消息");

                MapTask.TryGetValue(rpc_data.Ticket, out var t);
                if (t != null)
                {
                    MapTask.Remove(rpc_data.Ticket);
                    FreeTicket(rpc_data.Ticket);

                    t.Tcs.SetResult(rpc_data);
                }
            }
        }*/

        void OnWebSocketError(object sender, ErrorEventArgs e)
        {
            Debug.LogError($"RpcWebSocketClient.OnWebSocketError() Reason={e.Message}");

            ActionSocketError?.Invoke();
        }

        void OnWebSocketClosed(object sender, CloseEventArgs e)
        {
            Debug.Log($"RpcWebSocketClient.OnWebSocketClosed() Code={e.Code}, Message={e.Reason}");

            ActionSocketClosed?.Invoke();
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