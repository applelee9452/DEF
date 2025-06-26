namespace DEF.UCenter;

public class ContainerStatelessUCenter : ContainerStateless, IContainerStatelessUCenter
{
    public override Task OnCreate()
    {
        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    Task<AccountLoginResponse> IContainerStatelessUCenter.LoginRequest(AccountLoginInfo request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.LoginRequest(request, client_ip);
    }

    Task<UCenterErrorCode> IContainerStatelessUCenter.GetPhoneVerificationCodeRequest(GetPhoneVerificationCodeRequest request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.GetPhoneVerificationCodeRequest(request, client_ip);
    }

    Task<AccountRegisterResponse> IContainerStatelessUCenter.RegisterRequest(AccountRegisterRequestInfo request, ulong agent_id, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.RegisterRequest(request, agent_id, client_ip);
    }

    Task<AccountLoginResponse> IContainerStatelessUCenter.WechatAutoLoginRequest(AccountWechatAutoLoginRequest request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.WechatAutoLoginRequest(request, client_ip);
    }

    Task<AccountLoginResponse> IContainerStatelessUCenter.WechatLoginRequest(AccountWechatOAuthInfo request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.WechatLoginRequest(request, client_ip);
    }
    Task<AccountLoginResponse> IContainerStatelessUCenter.WechatMPLoginRequest(AccountWechatOAuthInfo request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.WechatMPLoginRequest(request, client_ip);
    }
    Task<AccountLoginResponse> IContainerStatelessUCenter.TaptapLoginRequest(AccountTaptapLoginRequest request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccountTaptap>();
        return c.TaptapLoginRequest(request, client_ip);
    }

    Task<AccountLoginResponse> IContainerStatelessUCenter.FacebookLoginRequest(AccountFacebookOAuthInfo request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.FacebookLoginRequest(request, client_ip);
    }

    Task<AccountLoginResponse> IContainerStatelessUCenter.GoogleLoginRequest(AccountGoogleOAuthInfo request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.GoogleLoginRequest(request, client_ip);
    }

    Task<AccountLoginResponse> IContainerStatelessUCenter.EnjoyLoginRequest(EnjoyLoginRequest request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.EnjoyLoginRequest(request, client_ip);
    }

    Task<GuestAccessResponse> IContainerStatelessUCenter.GuestAccessRequest(GuestAccessInfo request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.GuestAccessRequest(request, client_ip);
    }

    Task<PhoneVerificaitonCodeAccessResponse> IContainerStatelessUCenter.PhoneVerificationCodeAccessRequest(PhoneVerificaitonCodeAccessInfo request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.PhoneVertificationCodeAccessRequest(request, client_ip);
    }


    Task<AccountResetPasswordResponse> IContainerStatelessUCenter.ResetPasswordByPhoneRequest(AccountResetPasswordByPhoneRequest request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.ResetPasswordByPhoneRequest(request, client_ip);
    }

    Task<AccountChangeGenderResponse> IContainerStatelessUCenter.ChangeGenderRequest(AccountChangeGenderInfo request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.ChangeGenderRequest(request, client_ip);
    }

    Task<AccountUploadProfileImageResponse> IContainerStatelessUCenter.UploadProfileImageRequest(AccountUploadProfileImageRequest request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.UploadProfileImageRequest(request, client_ip);
    }

    Task<IdCardResponse> IContainerStatelessUCenter.IdCardCheckCardAndNameRequest(CheckCardAndNameRequest request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.IdCardCheckCardAndNameRequest(request, client_ip);
    }
    Task<IdCardResult> IContainerStatelessUCenter.IdCardResultRequest(DEF.UCenter.IDCradResultRequest request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.IdCardResultRequest(request, client_ip);
    }
    Task<SerializeObj<UCenterErrorCode, string>> IContainerStatelessUCenter.GetIPAddressRequest(IPCheckRequest request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.GetIPAddressRequest(request, client_ip);
    }
    Task<UCenterErrorCode> IContainerStatelessUCenter.ModifyPhoneNumber(ModifyPhoneNumberRequest request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.ModifyPhoneNumber(request, client_ip);
    }
    Task<GetPhoneNumberResponse> IContainerStatelessUCenter.GetPhoneNumber(GetPhoneNumberRequest request, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessAccount>();
        return c.GetPhoneNumber(request, client_ip);
    }

    // 请求创建代理账号
    async Task<ulong> IContainerStatelessUCenter.RequestCreateAgent(string user_name, ulong parent_agent_id)
    {
        var c = GetContainerRpc<IContainerStatelessAgent>();
        var data_agent = await c.CreateAgent(user_name, parent_agent_id);

        return data_agent.AgentId;
    }

    // 请求删除代理账号
    Task IContainerStatelessUCenter.RequestDeleteAgent(ulong agent_id)
    {
        var c = GetContainerRpc<IContainerStatelessAgent>();
        return c.RequestDeleteAgent(agent_id);
    }
}