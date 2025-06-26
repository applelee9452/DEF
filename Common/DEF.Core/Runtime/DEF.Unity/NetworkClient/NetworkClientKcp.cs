#if DEF_CLIENT

using System;
using kcp2k;
using UnityEngine;

namespace DEF
{
    public enum KcpErrorCode : byte
    {
        DnsResolve,       // failed to resolve a host name
        Timeout,          // ping timeout or dead link
        Congestion,       // more messages than transport / network can process
        InvalidReceive,   // recv invalid packet (possibly intentional attack)
        InvalidSend,      // user tried to send invalid data
        ConnectionClosed, // connection closed voluntarily or lost involuntarily
        Unexpected        // unexpected error / exception, requires fix.
    }

    public class NetworkClientKcp : MonoBehaviour
    {
        public bool IsConnected { get { return KcpClient != null && KcpClient.connected; } }
        public Action<byte[], int, int> OnRpcMethod { get; set; }
        public Action OnSocketConnected { get; set; }
        public Action OnSocketClosed { get; set; }
        public Action<KcpErrorCode, string> OnSocketError { get; set; }
        KcpClient KcpClient { get; set; }

        private void Update()
        {
            KcpClient?.Tick();
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        public void Connect(string host, int port)
        {
            Disconnect();

            int kcp_timeout = 10;// 默认10秒超时
#if UNITY_EDITOR
            kcp_timeout = 3600;// 编辑器下3600秒超时
#endif

            KcpConfig config = new(
            // force NoDelay and minimum interval.
            // this way UpdateSeveralTimes() doesn't need to wait very long and
            // tests run a lot faster.
            NoDelay: true,
            // not all platforms support DualMode.
            // run tests without it so they work on all platforms.
            DualMode: false,
            Interval: 1, // 1ms so at interval code at least runs.
            Timeout: kcp_timeout * 1000,
            RecvBufferSize: 1024 * 1027 * 7,
            SendBufferSize: 1024 * 1027 * 7,
            FastResend: 2,
            // large window sizes so large messages are flushed with very few
            // update calls. otherwise tests take too long.
            SendWindowSize: Kcp.WND_SND * 1000,
            ReceiveWindowSize: Kcp.WND_RCV * 1000,
            // congestion window _heavily_ restricts send/recv window sizes
            // sending a max sized message would require thousands of updates.
            CongestionWindow: false,
            // maximum retransmit attempts until dead_link detected
            // default * 2 to check if configuration works
            MaxRetransmits: Kcp.DEADLINK * 2
            );

            KcpClient = new(
                OnKcpConnected,
                OnKcpData,
                OnKcpClosed,
                OnKcpError,
                config);

            KcpClient.Connect(host, (ushort)port);
        }

        public void Disconnect()
        {
            KcpClient?.Disconnect();
        }

        public void Send(byte[] data)
        {
            KcpClient?.Send(data, KcpChannel.Reliable);
        }

        void OnKcpData(ArraySegment<byte> data, KcpChannel kcp_channel)
        {
            //Debug.Log($"OnKcpData Len={data.Count}");

            OnRpcMethod?.Invoke(data.Array, data.Offset, data.Count);
        }

        void OnKcpConnected()
        {
            OnSocketConnected?.Invoke();
        }

        void OnKcpClosed()
        {
            OnSocketClosed?.Invoke();
        }

        void OnKcpError(ErrorCode error_code, string reason)
        {
            Debug.LogError($"OnKcpError ErrorCode={error_code} reason={reason}");

            KcpErrorCode kcp_error_code = (KcpErrorCode)error_code;
            OnSocketError?.Invoke(kcp_error_code, reason);
        }
    }
}

#endif