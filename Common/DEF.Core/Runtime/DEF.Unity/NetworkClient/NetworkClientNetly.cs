#if DEF_CLIENT

using System;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using Netly;
using System.Threading;

namespace DEF
{
    public class NetworkClientNetly : MonoBehaviour
    {
        TCP.Client TcpClient { get; set; } = new();
        public Action<byte[]> OnRpcMethod { get; set; }
        public Action OnSocketConnected { get; set; }
        public Action OnSocketConnectFailed { get; set; }
        public Action OnSocketClosed { get; set; }
        public Action<SocketErrorEventArgs> OnSocketError { get; set; }
        TcpClientRecvData RecvData { get; set; }

        private void Awake()
        {
        }

        private async void OnDestroy()
        {
            await TcpClient.To.Close();

            OnSocketClosed?.Invoke();
        }

        public async void Connect(string host, int port)
        {
            RecvData = new TcpClientRecvData((byte[] data, int offset, int data_len) =>
            {
                OnRpcMethod?.Invoke(data);
            });

            TcpClient.On.Open(() =>
            {
                OnSocketConnected?.Invoke();
            });

            TcpClient.On.Close(() =>
            {
                OnSocketClosed?.Invoke();
            });

            TcpClient.On.Error(e =>
            {
                SocketErrorEventArgs args = new(e);
                OnSocketError?.Invoke(args);
            });

            TcpClient.On.Data(bytes =>
            {
                int num = bytes.Length;
                Array.Copy(bytes, RecvData.BufTmp, num);
                RecvData.AppendRecvData(num);
            });

            TcpClient.To.Encryption(true);
            TcpClient.On.Encryption((certificate, chain, errors) =>
            {
                return true;
            });

            Thread.Sleep(millisecondsTimeout: 1000);

            if (!IPAddress.TryParse(host, out IPAddress ip_address))
            {
                IPHostEntry host_info = Dns.GetHostEntry(host);
                IPAddress[] arr_ip = host_info.AddressList;
                ip_address = arr_ip[0];
            }

            await TcpClient.To.Open(new Host(ip_address, port));
        }

        public async Task Disconnect()
        {
            await TcpClient.To.Close();

            OnSocketClosed?.Invoke();
        }

        public void Send(byte[] data)
        {
            int len_int = sizeof(int);

            int data_len = 0;
            if (data != null && data.Length > 0)
            {
                data_len = data.Length;
            }

            byte[] send_bytes = new byte[len_int + data_len];

            Array.Copy(BitConverter.GetBytes(data_len), 0, send_bytes, 0, len_int);
            if (data != null && data.Length > 0) Array.Copy(data, 0, send_bytes, len_int, data_len);

            TcpClient.To.Data(send_bytes);
        }
    }
}

#endif