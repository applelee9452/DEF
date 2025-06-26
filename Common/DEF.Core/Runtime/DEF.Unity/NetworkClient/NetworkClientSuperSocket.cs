#if DEF_CLIENT

using System;
using System.Buffers;
using System.Net;
using System.Threading.Tasks;
using SuperSocket.Client;
using SuperSocket.ProtoBase;
using UnityEngine;

namespace DEF
{
    public class MyPackageFilter : FixedHeaderPipelineFilter<byte[]>
    {
        public MyPackageFilter()
            : base(4)
        {

        }

        protected override int GetBodyLengthFromHeader(ref ReadOnlySequence<byte> buffer)
        {
            var buffer2 = buffer.Slice(0, 4);
            var buffer3 = buffer2.ToArray();
            int data_len = BitConverter.ToInt32(buffer3);
            return data_len;
        }

        protected override byte[] DecodePackage(ref ReadOnlySequence<byte> buffer)
        {
            var reader = new SequenceReader<byte>(buffer);

            byte[] recv_data = reader.Sequence.Slice(4).ToArray();

            return recv_data;
        }
    }

    public class NetworkClientSuperSocket : MonoBehaviour
    {
        IEasyClient<byte[]> TcpClient { get; set; }
        public Action<byte[]> OnRpcMethod { get; set; }
        public Action OnSocketConnected { get; set; }
        public Action OnSocketConnectFailed { get; set; }
        public Action OnSocketClosed { get; set; }
        public Action<SocketErrorEventArgs> OnSocketError { get; set; }

        private void Awake()
        {
            TcpClient = new EasyClient<byte[]>(new MyPackageFilter()).AsClient();
        }

        private async void OnDestroy()
        {
            if (TcpClient != null)
            {
                await TcpClient.CloseAsync();
                TcpClient = null;

                OnSocketClosed?.Invoke();
            }
        }

        public async void Connect(string host, int port)
        {
            TcpClient.Closed += (object client, EventArgs args) =>
            {
                OnSocketClosed?.Invoke();
            };

            TcpClient.PackageHandler += (EasyClient<byte[]> sender, byte[] data) =>
            {
                OnRpcMethod?.Invoke(data);

                return new ValueTask();
            };

            if (!IPAddress.TryParse(host, out IPAddress ip_address))
            {
                IPHostEntry host_info = Dns.GetHostEntry(host);
                IPAddress[] arr_ip = host_info.AddressList;
                ip_address = arr_ip[0];
            }

            TcpClient.Security = new()
            {
                EnabledSslProtocols = System.Security.Authentication.SslProtocols.None,
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
            };
            bool b = await TcpClient.ConnectAsync(new IPEndPoint(ip_address, port));
            if (b)
            {
                // 连接成功

                TcpClient.StartReceive();

                OnSocketConnected?.Invoke();
            }
            else
            {
                // 连接失败

                OnSocketConnectFailed?.Invoke();
            }
        }

        public async Task Disconnect()
        {
            if (TcpClient != null)
            {
                await TcpClient.CloseAsync();
                TcpClient = null;

                OnSocketClosed?.Invoke();
            }
        }

        public async Task Send(byte[] data)
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

            await TcpClient.SendAsync(send_bytes);
        }
    }
}

#endif