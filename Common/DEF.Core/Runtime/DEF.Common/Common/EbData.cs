using System;
using System.IO;

namespace DEF
{
    public abstract class EbData
    {
        public int Id { get; internal set; }

        public abstract void Load(EbTableBuffer table_buf);
    }

    public class EbTableBuffer
    {
        public string TableName { get; private set; }
        MemoryStream MemoryStream { get; set; }

        byte[] BufferTmp;
        int ReadLen;
        int WriteLen;

        public EbTableBuffer(string tb_name)
        {
            MemoryStream = new MemoryStream();
            BufferTmp = new byte[1024];
            TableName = tb_name;
            ReadLen = 0;
            WriteLen = 0;
        }

        public EbTableBuffer(byte[] buf, string tb_name)
        {
            MemoryStream = new MemoryStream(buf);
            BufferTmp = new byte[1024];
            TableName = tb_name;
            ReadLen = 0;
            WriteLen = buf.Length;
        }

        public void Close()
        {
            if (MemoryStream != null)
            {
                MemoryStream.Close();
                MemoryStream = null;
            }
            BufferTmp = null;
            ReadLen = 0;
            WriteLen = 0;
        }

        public byte[] GetTableData()
        {
            return MemoryStream.ToArray();
        }

        public bool IsReadEnd()
        {
            return ReadLen >= WriteLen;
        }

        public void WriteInt(int value)
        {
            WriteLen += sizeof(int);
            var data = BitConverter.GetBytes(value);

            MemoryStream.Write(data, 0, data.Length);
        }

        public void WriteFloat(float value)
        {
            WriteLen += sizeof(float);
            var data = BitConverter.GetBytes(value);

            MemoryStream.Write(data, 0, data.Length);
        }

        public void WriteString(string value)
        {
            byte[] str_data = null;
            short str_len = 0;
            if (!string.IsNullOrEmpty(value))
            {
                str_data = System.Text.Encoding.UTF8.GetBytes(value);
                str_len = (short)str_data.Length;
            }

            WriteLen += sizeof(short);

            var short4len = BitConverter.GetBytes(str_len);
            MemoryStream.Write(short4len, 0, short4len.Length);

            if (str_len > 0)
            {
                WriteLen += str_len;
                MemoryStream.Write(str_data, 0, str_data.Length);
            }
        }

        public void WriteEnd()
        {
            MemoryStream.Seek(0, SeekOrigin.Begin);
        }

        public int ReadInt()
        {
            ReadLen += sizeof(int);

            MemoryStream.Read(BufferTmp, 0, sizeof(int));
            return BitConverter.ToInt32(BufferTmp, 0);
        }

        public float ReadFloat()
        {
            ReadLen += sizeof(float);

            MemoryStream.Read(BufferTmp, 0, sizeof(float));
            return BitConverter.ToSingle(BufferTmp, 0);
        }

        public string ReadString()
        {
            ReadLen += sizeof(short);

            MemoryStream.Read(BufferTmp, 0, sizeof(short));
            short short4len = BitConverter.ToInt16(BufferTmp, 0);

            if (short4len > 0)
            {
                if (short4len > BufferTmp.Length)
                {
                    BufferTmp = new byte[short4len + 128];
                }

                ReadLen += short4len;
                MemoryStream.Read(BufferTmp, 0, short4len);

                return System.Text.Encoding.UTF8.GetString(BufferTmp, 0, short4len);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}