using Microsoft.AspNetCore.Mvc;

namespace DEF.Gateway;

public class CCenterController : ControllerBase
{
    ILogger Logger { get; set; }
    ServiceClient ServiceClient { get; set; }

    public CCenterController(ILogger<CCenterController> logger, ServiceClient service_client)
        : base()
    {
        Logger = logger;
        ServiceClient = service_client;
    }

    [HttpPost]
    [Route("api/ccenter/getcfg")]
    public async Task<Dictionary<string, string>> GetCfg(string name_space, string bundle_version)
    {
        var container_ccenter = ServiceClient.GetContainerRpc<DEF.CCenter.IContainerStatelessCCenter>();
        var r = await container_ccenter.GetCfg(name_space, bundle_version);
        return r;
    }
}