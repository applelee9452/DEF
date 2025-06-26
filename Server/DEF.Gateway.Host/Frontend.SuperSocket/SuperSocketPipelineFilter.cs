using System.Buffers;
using SuperSocket.ProtoBase;

namespace DEF.Gateway;

public class SuperSocketPipelineFilter : FixedHeaderPipelineFilter<SuperSocketPacketInfo>
{
    public SuperSocketPipelineFilter()
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

    protected override SuperSocketPacketInfo DecodePackage(ref ReadOnlySequence<byte> buffer)
    {
        var packet = new SuperSocketPacketInfo { Data = buffer.Slice(4).ToArray() };
        return packet;
    }
}