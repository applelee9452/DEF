#if DEF_CLIENT

using System;
using UnityEngine;

namespace DEF
{
    public class TcpClientRecvData
    {
        public byte[] BufTmp { get; set; }
        byte[] Buf { get; set; }
        int WritePos { get; set; }
        int ReadPos { get; set; }
        int DataLen { get; set; }// DataLen不含自身的4字节
        Action<byte[], int, int> OnRpcMethod { get; set; }
        Action OnError { get; set; }

        public const int BufTmpLen = 1024 * 640;// 640Kb
        const int BufLen = 1024 * 256;// 256Kb
        //const int MaxDataLen = BufLen;// 256Kb

        public TcpClientRecvData(Action<byte[], int, int> handle_recvdata)
        {
            Buf = new byte[BufLen];
            BufTmp = new byte[BufTmpLen];
            OnRpcMethod = handle_recvdata;
            WritePos = 0;
            ReadPos = 0;
            DataLen = 0;
        }

        public void AppendRecvData(int read_num)
        {
            if (read_num == 0)
            {
                return;
            }

            if (read_num > BufTmpLen)
            {
                Debug.LogError($"TcpClientRecvData Error read_num={DataLen}，大于最大临时缓冲数据长度={BufTmpLen}");

                // error
                OnError?.Invoke();
                return;
            }

            // 追加数据
            Array.Copy(BufTmp, 0, Buf, WritePos, read_num);
            WritePos += read_num;

            //UnityEngine.Debug.Log("收到数据，DataLen=" + read_num);

            // 尝试递归解析数据包
            int len_int = sizeof(int);

            while (true)
            {
                int len_buf = WritePos - ReadPos;
                if (DataLen == 0)
                {
                    if (len_buf >= len_int)
                    {
                        DataLen = BitConverter.ToInt32(Buf, ReadPos);
                        if (DataLen > BufLen)
                        {
                            Debug.LogError($"TcpClientRecvData Error DataLen={DataLen}，大于最大数据长度={BufLen}");

                            // error
                            OnError?.Invoke();
                            break;
                        }
                    }
                }

                if (len_buf >= len_int + DataLen)
                {
                    OnRpcMethod?.Invoke(Buf, ReadPos + len_int, DataLen);

                    ReadPos += DataLen + len_int;
                    DataLen = 0;
                }
                else
                {
                    break;
                }
            }

            // 判定数据块是否到了末尾，如果到了末尾，则将数据块剪切到头部        
            if (BufLen - WritePos < BufTmpLen)
            {
                int writepos_new = 0;// 左数据块，右侧写位置
                int readpos_tmp = ReadPos;// 右数据块，左侧的读位置

                while (true)
                {
                    int len_buf = WritePos - ReadPos;
                    if (len_buf == 0)
                    {
                        ReadPos = 0;
                        WritePos = 0;
                        break;
                    }
                    else if (len_buf > BufTmpLen)
                    {
                        Array.Copy(Buf, readpos_tmp, BufTmp, 0, BufTmpLen);
                        Array.Copy(BufTmp, 0, Buf, writepos_new, BufTmpLen);

                        writepos_new += BufTmpLen;
                        readpos_tmp += BufTmpLen;
                    }
                    else
                    {
                        Array.Copy(Buf, readpos_tmp, BufTmp, 0, len_buf);
                        Array.Copy(BufTmp, 0, Buf, writepos_new, len_buf);

                        writepos_new += len_buf;

                        ReadPos = 0;
                        WritePos = writepos_new;
                        break;
                    }
                }
            }
        }
    }
}

#endif