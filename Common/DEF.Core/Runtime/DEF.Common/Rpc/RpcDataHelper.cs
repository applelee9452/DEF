using System;
using System.Text;

namespace DEF
{
    public class RpcData
    {
        public ushort Ticket;
        public bool HasResult;
        public string ServiceName;
        public ContainerStateType ContainerStateType;
        public string ContainerType;
        public string ContainerId;
        public long EntityId;
        public string ComponentName;
        public string MethodName;
        public byte[] MethodData;
        public int MethodDataLen;
        public int TotalDataLen;
    }

    public static class RpcDataHelper
    {
        public static byte[] Pack(RpcData rpc_data, bool contain_fixedhead = false)
        {
            return Pack(
                rpc_data.Ticket, rpc_data.HasResult,
                rpc_data.ServiceName,
                (int)rpc_data.ContainerStateType,
                rpc_data.ContainerType, rpc_data.ContainerId,
                rpc_data.EntityId, rpc_data.ComponentName,
                rpc_data.MethodName, rpc_data.MethodData,
                contain_fixedhead);
        }

        public static byte[] Pack(
            ushort ticket,
            bool has_result,
            string service_name,
            int container_state_type,
            string container_type,
            string container_id,
            long entity_id,
            string component_name,
            string method_name, byte[] method_data,
            bool contain_fixedhead)
        {
            int total_len = 0;

            var bytes_ticket = BitConverter.GetBytes(ticket);
            total_len += bytes_ticket.Length;

            var bytes_hasresult = BitConverter.GetBytes(has_result);
            total_len += bytes_hasresult.Length;

            total_len += GetStringLen(service_name);

            var bytes_container_state_type = BitConverter.GetBytes(container_state_type);
            total_len += bytes_container_state_type.Length;

            total_len += GetStringLen(container_type);

            total_len += GetStringLen(container_id);

            var bytes_entity_id = BitConverter.GetBytes(entity_id);
            total_len += bytes_entity_id.Length;

            total_len += GetStringLen(component_name);

            total_len += GetStringLen(method_name);

            if (method_data != null)
            {
                total_len += method_data.Length;
            }

            byte[] buff;
            int offset = 0;

            if (contain_fixedhead)
            {
                buff = new byte[total_len + 4];
                byte[] arr_fixedhead = BitConverter.GetBytes(total_len);

                arr_fixedhead.CopyTo(buff, offset);
                offset += 4;
            }
            else
            {
                buff = new byte[total_len];
            }

            bytes_ticket.CopyTo(buff, offset);
            offset += bytes_ticket.Length;

            bytes_hasresult.CopyTo(buff, offset);
            offset += bytes_hasresult.Length;

            offset += WriteString(buff, offset, service_name);

            bytes_container_state_type.CopyTo(buff, offset);
            offset += bytes_container_state_type.Length;

            offset += WriteString(buff, offset, container_type);

            offset += WriteString(buff, offset, container_id);

            bytes_entity_id.CopyTo(buff, offset);
            offset += bytes_entity_id.Length;

            offset += WriteString(buff, offset, component_name);

            offset += WriteString(buff, offset, method_name);

            if (method_data != null)
            {
                method_data.CopyTo(buff, offset);
                offset += method_data.Length;
            }

            return buff;
        }

        static int GetStringLen(string s)
        {
            int len = 0;

            byte[] bytes_s = null;
            if (!string.IsNullOrEmpty(s))
            {
                bytes_s = Encoding.UTF8.GetBytes(s);
            }
            ushort len_s = bytes_s == null ? (ushort)0 : (ushort)bytes_s.Length;
            var bytes_len_s = BitConverter.GetBytes(len_s);
            len += bytes_len_s.Length;
            len += bytes_s == null ? 0 : bytes_s.Length;

            return len;
        }

        static int WriteString(byte[] buff, int offset, string s)
        {
            int len = 0;

            byte[] bytes_container_id = null;
            if (!string.IsNullOrEmpty(s))
            {
                bytes_container_id = Encoding.UTF8.GetBytes(s);
            }

            ushort len_container_id = bytes_container_id == null ? (ushort)0 : (ushort)bytes_container_id.Length;
            var bytes_len_container_id = BitConverter.GetBytes(len_container_id);
            bytes_len_container_id.CopyTo(buff, offset);
            offset += bytes_len_container_id.Length;
            len += bytes_len_container_id.Length;

            if (bytes_container_id != null)
            {
                bytes_container_id.CopyTo(buff, offset);
                offset += bytes_container_id.Length;
                len += bytes_container_id.Length;
            }

            return len;
        }

        public static RpcData UnPack(byte[] buff, int offset, int len)
        {
            //System.Net.IPAddress.NetworkToHostOrder
            RpcData rpc_data = new();

            rpc_data.TotalDataLen = len;
            int offset_original = offset;

            rpc_data.Ticket = BitConverter.ToUInt16(buff, offset);
            offset += sizeof(ushort);

            rpc_data.HasResult = BitConverter.ToBoolean(buff, offset);
            offset += sizeof(bool);

            var len_service_name = BitConverter.ToUInt16(buff, offset);
            offset += sizeof(ushort);
            rpc_data.ServiceName = Encoding.UTF8.GetString(buff, offset, len_service_name);
            offset += len_service_name;

            rpc_data.ContainerStateType = (ContainerStateType)BitConverter.ToInt32(buff, offset);
            offset += sizeof(int);

            var len_container_type = BitConverter.ToUInt16(buff, offset);
            offset += sizeof(ushort);
            rpc_data.ContainerType = Encoding.UTF8.GetString(buff, offset, len_container_type);
            offset += len_container_type;

            var len_container_id = BitConverter.ToUInt16(buff, offset);
            offset += sizeof(ushort);
            rpc_data.ContainerId = Encoding.UTF8.GetString(buff, offset, len_container_id);
            offset += len_container_id;

            rpc_data.EntityId = BitConverter.ToInt64(buff, offset);
            offset += sizeof(long);

            var len_component_name = BitConverter.ToUInt16(buff, offset);
            offset += sizeof(ushort);
            rpc_data.ComponentName = Encoding.UTF8.GetString(buff, offset, len_component_name);
            offset += len_component_name;

            var len_method_name = BitConverter.ToUInt16(buff, offset);
            offset += sizeof(ushort);
            rpc_data.MethodName = Encoding.UTF8.GetString(buff, offset, len_method_name);
            offset += len_method_name;

            if (len == offset - offset_original)
            {
                rpc_data.MethodData = null;
                rpc_data.MethodDataLen = 0;
            }
            else
            {
                rpc_data.MethodDataLen = len - (offset - offset_original);
                rpc_data.MethodData = new byte[rpc_data.MethodDataLen];
                Array.Copy(buff, offset, rpc_data.MethodData, 0, rpc_data.MethodDataLen);
            }

            rpc_data.ServiceName = rpc_data.ServiceName.ToLower();

            return rpc_data;
        }

#if !DEF_CLIENT
    public static RpcData UnPack(DotNetty.Buffers.IByteBuffer buff)
    {
        RpcData rpc_data = new();

        rpc_data.TotalDataLen = buff.ReadableBytes;

        rpc_data.Ticket = buff.ReadUnsignedShortLE();

        rpc_data.HasResult = buff.ReadBoolean();

        var len_service_name = buff.ReadUnsignedShortLE();
        if (len_service_name > 0)
        {
            rpc_data.ServiceName = buff.ReadString(len_service_name, Encoding.UTF8);
        }

        rpc_data.ContainerStateType = (ContainerStateType)buff.ReadIntLE();
        
        var len_container_type = buff.ReadUnsignedShortLE();
        if (len_container_type > 0)
        {
            rpc_data.ContainerType = buff.ReadString(len_container_type, Encoding.UTF8);
        }

        var len_container_id = buff.ReadUnsignedShortLE();
        if (len_container_id > 0)
        {
            rpc_data.ContainerId = buff.ReadString(len_container_id, Encoding.UTF8);
        }
        
        rpc_data.EntityId = buff.ReadLongLE();
        
        var len_component_name = buff.ReadUnsignedShortLE();
        if (len_component_name > 0)
        {
            rpc_data.ComponentName = buff.ReadString(len_component_name, Encoding.UTF8);
        }

        var len_method_name = buff.ReadUnsignedShortLE();
        if (len_method_name > 0)
        {
            rpc_data.MethodName = buff.ReadString(len_method_name, Encoding.UTF8);
        }

        if (buff.ReadableBytes == 0)
        {
            rpc_data.MethodData = null;
        }
        else
        {
            rpc_data.MethodDataLen = buff.ReadableBytes;
            rpc_data.MethodData = new byte[buff.ReadableBytes];
            buff.ReadBytes(rpc_data.MethodData);
        }

        rpc_data.ServiceName = rpc_data.ServiceName.ToLower();

        return rpc_data;
    }
#endif
    }
}