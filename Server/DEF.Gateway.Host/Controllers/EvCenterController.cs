using DEF.EvCenter;
using Microsoft.AspNetCore.Mvc;

namespace DEF.Gateway;

public class EvCenterController : ControllerBase
{
    ILogger Logger { get; set; }
    ServiceClient ServiceClient { get; set; }

    public EvCenterController(ILogger<GatewayController> logger, ServiceClient service_client)
        : base()
    {
        Logger = logger;
        ServiceClient = service_client;
    }

    [HttpPost]
    [Route("api/evcenter/clientexception")]
    public async Task ClientException()
    {
        var client_ip = ControllerContext.HttpContext.GetClientIpAddress();

        Logger.LogDebug("EvCenterController.ClientException() ClientIp={ClientIp}", client_ip);

        int len = (int)Request.ContentLength;
        byte[] buf = new byte[len];
        await Request.Body.ReadExactlyAsync(buf.AsMemory(0, len));
        byte[] data = buf;

        if (data != null)
        {
            using var ms = new MemoryStream(data);
            var info = ProtoBuf.Serializer.Deserialize<CrashReportInfo>(ms);

            var c = ServiceClient.GetContainerRpc<DEF.EvCenter.IContainerStatelessEvCenter>();
            await c.ClientCrashReport(info, client_ip);
        }
    }
}