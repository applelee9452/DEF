#if DEF_CLIENT

using System;
using UnityEngine;

namespace DEF
{
    public class NetworkClientTcp : MonoBehaviour
    {
        TcpClient4DEF TcpClient { get; set; }
        public Action<byte[], int, int> OnRpcMethod { get; set; }
        public Action OnSocketConnected { get; set; }
        public Action OnSocketConnectFailed { get; set; }
        public Action OnSocketClosed { get; set; }
        public Action<SocketErrorEventArgs> OnSocketError { get; set; }
        public bool IsConnected { get { return TcpClient != null && TcpClient.IsConnected; } }

        private void Update()
        {
            TcpClient?.Update();
        }

        private void OnDestroy()
        {
            if (TcpClient != null)
            {
                TcpClient.Close();
                TcpClient = null;
            }
        }

        public void Connect(string host, int port)
        {
            TcpClient ??= new TcpClient4DEF
            {
                OnSocketReceive = OnRpcMethod,
                OnSocketConnected = (object client, EventArgs args) => { OnSocketConnected?.Invoke(); },
                OnSocketConnectFailed = (object client, EventArgs args) => { OnSocketConnectFailed?.Invoke(); },
                OnSocketClosed = (object client, EventArgs args) => { OnSocketClosed?.Invoke(); },
                OnSocketError = (object rec, SocketErrorEventArgs args) => { OnSocketError?.Invoke(args); },
            };

            if (TcpClient.IsConnected)
            {
                TcpClient.Disconnect();
            }

            TcpClient.Connect(host, port);
        }

        public void Disconnect()
        {
            if (TcpClient != null)
            {
                TcpClient.Close();
                TcpClient = null;
            }
        }

        public void Send(byte[] data)
        {
            TcpClient?.Send(data);
        }
    }
}

#endif