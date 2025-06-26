#if DEF_CLIENT

using System.Collections.Generic;

namespace DEF
{
    public class SendData
    {
        public byte[] Data;
        public int DataLen;
        public bool Pooled;
        public const int MaxLen = 1024 * 8;// 8Kb
    }

    public class TcpClientSendData
    {
        readonly Queue<SendData> QueSending = new();
        readonly Queue<SendData> QueSendPool = new();

        public int SendLength { get; private set; } = 0;
        public bool HaveData { get { return QueSending.Count > 0; } }

        public SendData GenSendData(int need_len)
        {
            if (need_len > SendData.MaxLen - 4)
            {
                var send_data = new SendData()
                {
                    Data = new byte[need_len + 4],
                    Pooled = false,
                    DataLen = 0
                };
                return send_data;
            }

            if (QueSendPool.Count > 0)
            {
                return QueSendPool.Dequeue();
            }
            else
            {
                var send_data = new SendData()
                {
                    Data = new byte[SendData.MaxLen],
                    Pooled = true,
                    DataLen = 0
                };
                return send_data;
            }
        }

        public void Send(SendData send_data)
        {
            QueSending.Enqueue(send_data);
        }

        public SendData PeekData()
        {
            return QueSending.Peek();
        }

        public void AfterSendData(SendData send_data, int send_num)
        {
            if (send_num > 0)
            {
                SendLength += send_num;
                if (SendLength >= send_data.DataLen)
                {
                    if (send_data.Pooled)
                    {
                        QueSendPool.Enqueue(send_data);
                    }

                    QueSending.Dequeue();
                    SendLength = 0;
                }
            }
        }
    }
}

#endif