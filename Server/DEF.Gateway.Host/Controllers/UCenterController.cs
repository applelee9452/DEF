using DEF.UCenter;
using Microsoft.AspNetCore.Mvc;

namespace DEF.Gateway;

public class UCenterController : ControllerBase
{
    ILogger Logger { get; set; }
    ServiceClient ServiceClient { get; set; }

    public UCenterController(ILogger<UCenterController> logger, ServiceClient service_client)
        : base()
    {
        Logger = logger;
        ServiceClient = service_client;
    }

    // 测试
    [HttpGet]
    [Route("api/ucenter/test")]
    public Task<int> Test(string s)
    {
        Logger.LogDebug("Test，s={s}", s);

        return Task.FromResult(100);
    }

    // 获取验证码
    [HttpPost]
    [Route("api/ucenter/getphonevertificationcode")]
    public async Task<DEF.UCenter.PhoneVerificaitonCodeAccessResponse> GetPhoneVertificationCode([FromBody] DEF.UCenter.GetPhoneVerificationCodeRequest request)
    {
        var container_ucenter = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessUCenter>();
        var guestaccess_response_code = await container_ucenter.GetPhoneVerificationCodeRequest(request, "");

        DEF.UCenter.PhoneVerificaitonCodeAccessResponse response = new()
        {
            ErrorCode = guestaccess_response_code
        };

        return response;
    }

    // 登陆
    //[HttpPost]
    //[Route("api/ucenter/phonevertificationcodeaccessrequest")]
    //public async Task<DEF.UCenter.PhoneVerificaitonCodeAccessResponse> phonevertificationcodeaccessrequest([FromBody] DEF.UCenter.WebPhoneVerificationLoginRequest request)
    //{
    //    PhoneVerificaitonCodeAccessInfo accessRequestInfo = new()
    //    {
    //        AppId = "Guandan",
    //        PhoneCode = "86",
    //        PhoneNum = request.PhoneNumber,
    //        Device = new()
    //        {
    //            Id = Guid.NewGuid().ToString(),
    //            Name = "webview",
    //            Type = "web",
    //            Model = "chrome",
    //            OperationSystem = "websystem",
    //        },
    //        VertificationCode = request.PhoneVertificationCode,
    //    };

    //    var container_ucenter = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessUCenter>();
    //    return await container_ucenter.PhoneVerificationCodeAccessRequest(accessRequestInfo, string.Empty);
    //}

    // 注册账号
    [HttpPost]
    [Route("api/ucenter/register")]
    public Task<DEF.UCenter.AccountRegisterResponse> Register([FromBody] DEF.UCenter.AccountRegisterRequestInfo request, [FromQuery] ulong agent_id)
    {
        Logger.LogDebug("UCenterController.Register() AccountName={request.AccountName} AgentId={AgentId}", request.AccountName, agent_id);

        var client_ip = ControllerContext.HttpContext.GetClientIpAddress();

        var c = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessUCenter>();
        return c.RegisterRequest(request, agent_id, client_ip);
    }

    // 获取商户指定日期推广的用户总数
    [HttpPost]
    [Route("api/ucenter/merchantobtainusercount")]
    public Task<int> MerchantObtainUserCount(string auth, DateTime begin_time, DateTime end_time)
    {
        var container_merchant = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessMerchant>();
        return container_merchant.MerchantObtainUserCount(auth, begin_time, end_time);
    }

    // 获取商户指定日期推广的用户列表，分页的，每页最多100条，需要指定页码，页码下标从0开始
    [HttpPost]
    [Route("api/ucenter/merchantobtainuserlist")]
    public Task<List<DEF.UCenter.MerchantAccount>> MerchantObtainUserList(string auth, int page_index, DateTime begin_time, DateTime end_time)
    {
        var container_merchant = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessMerchant>();
        return container_merchant.MerchantObtainUserList(auth, page_index, begin_time, end_time);
    }

    // 获取商户的一级代理商的总数
    [HttpPost]
    [Route("api/ucenter/merchantgettopusercount")]
    public Task<int> MerchantGetTopUserCount(string auth, DateTime begin_time, DateTime end_time)
    {
        var container_merchant = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessMerchant>();
        return container_merchant.MerchantGetTopUserCount(auth, begin_time, end_time);
    }

    // 获取商户指定日期推广的一级代理商列表，分页的，每页最多100条，需要指定页码，页码下标从0开始
    [HttpPost]
    [Route("api/ucenter/merchantgettopuserlist")]
    public Task<List<DEF.UCenter.MerchantAccount>> MerchantGetTopUserList(string auth, int page_index, DateTime begin_time, DateTime end_time)
    {
        var container_merchant = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessMerchant>();
        return container_merchant.MerchantGetTopUserList(auth, page_index, begin_time, end_time);
    }

    // 获取商户的指定日期的指定代理商的直接用户的总数
    [HttpPost]
    [Route("api/ucenter/merchantgetchildrencount")]
    public Task<int> MerchantGetChildrenCount(string auth, string agent_id, DateTime begin_time, DateTime end_time)
    {
        var container_merchant = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessMerchant>();
        return container_merchant.MerchantGetChildrenCount(auth, agent_id, begin_time, end_time);
    }

    // 获取商户指定日期的指定代理商的直接用户总数，分页的，每页最多100条，需要指定页码，页码下标从0开始
    [HttpPost]
    [Route("api/ucenter/merchantgetchildrenlist")]
    public Task<List<DEF.UCenter.MerchantAccount>> MerchantGetChildrenList(string auth, string agent_id, int page_index, DateTime begin_time, DateTime end_time)
    {
        var container_merchant = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessMerchant>();
        return container_merchant.MerchantGetChildrenList(auth, agent_id, page_index, begin_time, end_time);
    }

    // 获取商户指定日期的所有用户的充值笔数
    [HttpPost]
    [Route("api/ucenter/merchantgetuserrechargecount")]
    public Task<int> MerchantGetUserRechargeCount(string auth, DateTime begin_time, DateTime end_time)
    {
        var container_merchant = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessMerchant>();
        return container_merchant.MerchantGetUserRechargeCount(auth, begin_time, end_time);
    }

    // 获取商户指定日期的所有用户的充值列表，分页的，每页最多100条，需要指定页码，页码下标从0开始
    [HttpPost]
    [Route("api/ucenter/merchantgetuserrechargelist")]
    public Task<List<DEF.UCenter.MerchantAccountRecharge>> MerchantGetUserRechargeList(string auth, int page_index, DateTime begin_time, DateTime end_time)
    {
        var container_merchant = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessMerchant>();
        return container_merchant.MerchantGetUserRechargeList(auth, page_index, begin_time, end_time);
    }
}