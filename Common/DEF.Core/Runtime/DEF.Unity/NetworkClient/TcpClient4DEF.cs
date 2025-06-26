#if DEF_CLIENT

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace DEF
{
    enum SocketEventType : byte
    {
        Null = 0,
        Connected,
        ConnectFailed,
        Closed,
        Error,
    }

    struct SocketEvent
    {
        public SocketEventType type;
        public object client;
        public EventArgs args;
    }

    public class SocketErrorEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public SocketErrorEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }

    public delegate void OnSocketConnected(object client, EventArgs args);
    public delegate void OnSocketConnectFailed(object client, EventArgs args);
    public delegate void OnSocketClosed(object client, EventArgs args);
    public delegate void OnSocketError(object rec, SocketErrorEventArgs args);

    public class TcpClient4DEF : IDisposable
    {
        public bool IsConnected { get { return (Socket == null || Disposed) ? false : Socket.Connected; } }
        public Action<byte[], int, int> OnSocketReceive { get; set; }
        public OnSocketConnected OnSocketConnected { get; set; }
        public OnSocketConnectFailed OnSocketConnectFailed { get; set; }
        public OnSocketClosed OnSocketClosed { get; set; }
        public OnSocketError OnSocketError { get; set; }
        string Host { get; set; }
        int Port { get; set; }
        ConcurrentQueue<SocketEvent> QueSocketEvent { get; set; } = new ConcurrentQueue<SocketEvent>();
        TcpClientSendData SendData { get; set; }
        TcpClientRecvData RecvData { get; set; }

        Socket Socket;
        volatile bool Disposed = false;

        public TcpClient4DEF()
        {
            SendData = new TcpClientSendData();
        }

        ~TcpClient4DEF()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (Socket != null)
                        {
                            Socket.Shutdown(SocketShutdown.Both);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        try
                        {
                            if (Socket != null)
                            {
                                QueSocketEvent.Clear();
                                Socket.Close();
                                Socket = null;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                Disposed = true;
            }
        }

        public void Connect(string host, int port)
        {
            RecvData = new TcpClientRecvData(OnSocketReceive);

            Host = host;
            Port = port;
            Disposed = false;

            if (!IPAddress.TryParse(Host, out IPAddress ip_address))
            {
                IPHostEntry host_info = Dns.GetHostEntry(Host);
                IPAddress[] arr_ip = host_info.AddressList;
                ip_address = arr_ip[0];
            }

            if (ip_address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }

            Socket.BeginConnect(ip_address, Port, new AsyncCallback(_onConnect), Socket);
        }

        public void Disconnect()
        {
            if (Socket != null && Socket.Connected)
            {
                Socket.Disconnect(false);
                Socket.Dispose();
                Socket = null;
            }
        }

        public void Update()
        {
            if (QueSocketEvent.Count > 0)
            {
                SocketEvent socket_event;
                socket_event.type = SocketEventType.Null;
                socket_event.client = null;
                socket_event.args = null;

                if (QueSocketEvent.TryDequeue(out socket_event))
                {
                    if (socket_event.type == SocketEventType.Null) return;

                    switch (socket_event.type)
                    {
                        case SocketEventType.Connected:
                            {
                                OnSocketConnected?.Invoke(null, socket_event.args);
                            }
                            break;
                        case SocketEventType.ConnectFailed:
                            {
                                OnSocketConnectFailed?.Invoke(null, socket_event.args);
                            }
                            break;
                        case SocketEventType.Closed:
                            {
                                OnSocketClosed?.Invoke(null, socket_event.args);
                            }
                            break;
                        case SocketEventType.Error:
                            {
                                OnSocketError?.Invoke(null, (SocketErrorEventArgs)socket_event.args);
                            }
                            break;
                    }
                }
            }

            if (Socket != null && Socket.Connected)
            {
                try
                {
                    //if (Disposed) return;
                    //if (Disposed || !Socket.Connected) return;

                    if (Socket.Poll(0, SelectMode.SelectError))
                    {
                        //Debug.Log("SelectError~~~~~~");

                        _onError(null, new Exception("SocketError"));
                        //Dispose();
                        return;
                    }

                    if (Socket.Poll(0, SelectMode.SelectRead))
                    {
                        int num = Socket.Receive(RecvData.BufTmp, TcpClientRecvData.BufTmpLen, SocketFlags.None);
                        RecvData.AppendRecvData(num);
                    }

                    if (Socket.Poll(0, SelectMode.SelectWrite))
                    {
                        if (SendData.HaveData)
                        {
                            var send_data = SendData.PeekData();
                            int send_num = Socket.Send(send_data.Data, SendData.SendLength, send_data.DataLen - SendData.SendLength, SocketFlags.None);
                            SendData.AfterSendData(send_data, send_num);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("TcpClient4DEF.Update() \n" + e.StackTrace);

                    _onError(null, e);
                    //Dispose();
                }
            }
        }

        public void Close()
        {
            Dispose();

            OnSocketClosed?.Invoke(null, EventArgs.Empty);
        }

        public void Send(byte[] data)
        {
            SendData send_data = SendData.GenSendData(data != null ? data.Length : 0);

            int len_int = sizeof(int);
            int data_len = 0;
            if (data != null && data.Length > 0)
            {
                data_len += data.Length;
            }

            Array.Copy(BitConverter.GetBytes(data_len), 0, send_data.Data, 0, len_int);
            if (data != null && data.Length > 0) Array.Copy(data, 0, send_data.Data, len_int, data.Length);

            send_data.DataLen = len_int + data_len;

            SendData.Send(send_data);
        }

        void _onConnect(IAsyncResult result)
        {
            try
            {
                Socket socket = (Socket)result.AsyncState;
                socket.EndConnect(result);

                if (socket.Connected)
                {
#if !__IOS__
                    Socket.NoDelay = true;
#endif
                    Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

                    _onConnected(null, EventArgs.Empty);
                }
                else
                {
                    _onConnectFailed(null, EventArgs.Empty);
                }
            }
            catch (Exception)
            {
                _onConnectFailed(null, EventArgs.Empty);
            }
        }

        void _onConnected(object client, EventArgs args)
        {
            SocketEvent socket_event;
            socket_event.type = SocketEventType.Connected;
            socket_event.client = client;
            socket_event.args = args;

            QueSocketEvent.Enqueue(socket_event);
        }

        void _onConnectFailed(object client, EventArgs args)
        {
            SocketEvent socket_event;
            socket_event.type = SocketEventType.ConnectFailed;
            socket_event.client = client;
            socket_event.args = args;

            QueSocketEvent.Enqueue(socket_event);
        }

        void _onClosed(object client, EventArgs args)
        {
            SocketEvent socket_event;
            socket_event.type = SocketEventType.Closed;
            socket_event.client = client;
            socket_event.args = args;

            QueSocketEvent.Enqueue(socket_event);
        }

        void _onError(object client, Exception e)
        {
            SocketErrorEventArgs args = new SocketErrorEventArgs(e);

            SocketEvent socket_event;
            socket_event.type = SocketEventType.Error;
            socket_event.client = client;
            socket_event.args = args;

            QueSocketEvent.Enqueue(socket_event);
        }
    }
}

#endif