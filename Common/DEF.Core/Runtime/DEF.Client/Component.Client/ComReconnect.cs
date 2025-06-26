#if DEF_CLIENT

using DEF;
using System.Collections.Generic;
using UnityEngine;

namespace DanMu
{
    // 客户端网络断线重连组件。状态机
    [ComponentImpl]
    public class ComReconnect : ComponentLocal
    {
        public enum ConnectState
        {
            Disconnected = 0,// 已断开
            Connecting,// 连接中
            Connected// 已连接
        }

        public System.Action OnReconnecting { get; set; }// 通知回调
        RpcKcpClient RpcKcpClient { get; set; }
        RpcTcpClient RpcTcpClient { get; set; }
        RpcSuperSocketClient RpcSuperSocketClient { get; set; }
        RpcNetlyClient RpcNetlyClient { get; set; }
        EbTimer TimerUpdateHandler { get; set; }
        ConnectState State { get; set; }
        bool OpenReconnect { get; set; } = false;// 是否开启断线重连
        float WaitingReconnectLeftTm { get; set; } = 0f;// 等待下次重连剩余时间

        public override void Awake(Dictionary<string, object> create_params)
        {
            Debug.Log($"ComReconnect.Awake()");
        }

        public override void OnStart()
        {
            Debug.Log($"ComReconnect.OnStart()");

            TimerUpdateHandler ??= TimerShaft.RegisterTimer(200, (tm) =>
            {
                if (!OpenReconnect) return;

                switch (State)
                {
                    case ConnectState.Disconnected:
                        if (WaitingReconnectLeftTm <= 0f)
                        {
                            State = ConnectState.Connecting;
                            WaitingReconnectLeftTm = 3f;

                            OnReconnecting?.Invoke();

                            // 开始重连

                            RpcTcpClient?.Connect();

                            RpcSuperSocketClient?.Connect();

                            RpcNetlyClient?.Connect();

                            RpcKcpClient?.Connect();
                        }
                        else
                        {
                            WaitingReconnectLeftTm -= tm;
                        }
                        break;

                    case ConnectState.Connecting:
                        break;
                    case ConnectState.Connected:
                        OpenReconnect = false;
                        break;
                }
            });
        }

        public override void OnDestroy(string reason = null, byte[] user_data = null)
        {
            if (TimerUpdateHandler != null)
            {
                TimerUpdateHandler.Close();
                TimerUpdateHandler = null;
            }

            UnListenAllEvent();

            Debug.Log($"ComReconnect.OnDestroy");
        }

        public override void HandleSelfEvent(DEF.SelfEvent ev)
        {
        }

        public override void HandleEvent(DEF.Event ev)
        {
        }

        // 重连成功，关闭重连
        public void ReconnectSuccess()
        {
            OpenReconnect = false;
            State = ConnectState.Connected;
            WaitingReconnectLeftTm = 3f;
        }

        // 重连失败或者连接异常，等待几秒后开始下一次重连
        public void ReconnectFailedOrError(RpcTcpClient rpc_tcp_client, RpcSuperSocketClient rpc_supersocket_client, RpcNetlyClient rpc_netly_client, RpcKcpClient rpc_kcp_client)
        {
            RpcKcpClient = rpc_kcp_client;
            RpcTcpClient = rpc_tcp_client;
            RpcSuperSocketClient = rpc_supersocket_client;
            RpcNetlyClient = rpc_netly_client;
            OpenReconnect = true;
            State = ConnectState.Disconnected;
            WaitingReconnectLeftTm = 3f;
        }
    }
}

#endif