

using DotNetty.Buffers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DEF.Gateway;

public class GatewayNotifyService : NotifySDK.NotifySDKBase
{
    private readonly ILogger _logger;

    public GatewayNotifyService(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<GatewayNotifyService>();
    }

    public override Task<Empty> Notify(NotifyRequest request, ServerCallContext context)
    {
        byte[] method_data = request.Data == Google.Protobuf.ByteString.Empty ? null : request.Data.ToByteArray();

        //if (method_data == null)
        //{
        //    _logger.LogInformation("GatewayNotifyService.Notify() MethodId={0} DataLen=0", request.Id);
        //}
        //else
        //{
        //    _logger.LogInformation("GatewayNotifyService.Notify() MethodId={0} DataLen={1}", request.Id, method_data.Length);
        //}

        //if (GatewayContext.Instance.MapDotNettyChannelHandler.TryGetValue(request.SessionGuid, out var chc))
        //{
        //    int len = sizeof(ushort) * 2;

        //    if (method_data != null)
        //    {
        //        len += method_data.Length;
        //    }

        //    IByteBuffer msg1 = chc.Allocator.Buffer(len, len);
        //    msg1.WriteUnsignedShortLE((ushort)request.Id);
        //    msg1.WriteUnsignedShortLE((ushort)request.ServiceType);
        //    if (method_data != null)
        //    {
        //        msg1.WriteBytes(method_data);
        //    }
        //    chc.WriteAndFlushAsync(msg1);
        //}

        return Task.FromResult(new Empty { });
    }
}