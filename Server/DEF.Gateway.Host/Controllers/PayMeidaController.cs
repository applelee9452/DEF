#nullable enable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DEF.Gateway;

// 支付，美达渠道回调
[AllowAnonymous]
public class PayMeidaController : ControllerBase
{
    ILogger Logger { get; set; }
    ServiceClient ServiceClient { get; set; }

    public PayMeidaController(ILogger<PayMeidaController> logger, ServiceClient service_client)
        : base()
    {
        Logger = logger;
        ServiceClient = service_client;
    }

    [HttpGet()]
    [Route("api/meidacallback")]
    public async Task<IActionResult> MeidaCallback()
    {
        IQueryCollection qc = Request.Query;
        if (qc.Count == 0)
        {
            return Ok("No params input");
        }

        var list_kv = qc.Select(i => new KeyValuePair<string, string?>(i.Key, i.Value)).ToDictionary<string, string>();

        var c = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessPayCenter>();
        string result = await c.MeidaWebhook(list_kv);

        return Ok(result);
    }
}
