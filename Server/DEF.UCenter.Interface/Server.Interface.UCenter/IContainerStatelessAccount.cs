namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "Account", ContainerStateType.Stateless)]
public interface IContainerStatelessAccount : IContainerRpc
{
    // 请求账号登陆
    Task<AccountLoginResponse> LoginRequest(AccountLoginInfo request, string client_ip);

    // 请求获取手机验证码
    Task<UCenterErrorCode> GetPhoneVerificationCodeRequest(GetPhoneVerificationCodeRequest request, string client_ip);

    // 请求注册账号
    Task<AccountRegisterResponse> RegisterRequest(AccountRegisterRequestInfo request, ulong agent_id, string client_ip);

    // 请求微信自动登陆
    Task<AccountLoginResponse> WechatAutoLoginRequest(AccountWechatAutoLoginRequest request, string client_ip);

    // 请求微信登陆
    Task<AccountLoginResponse> WechatLoginRequest(AccountWechatOAuthInfo request, string client_ip);

    // 请求微信公众号登陆
    Task<AccountLoginResponse> WechatMPLoginRequest(AccountWechatOAuthInfo request, string client_ip);

    // 请求Facebook登陆
    Task<AccountLoginResponse> FacebookLoginRequest(AccountFacebookOAuthInfo request, string client_ip);

    // 请求Google登陆
    Task<AccountLoginResponse> GoogleLoginRequest(AccountGoogleOAuthInfo request, string client_ip);

    // 请求Enjoy登陆
    Task<AccountLoginResponse> EnjoyLoginRequest(EnjoyLoginRequest request, string client_ip);

    // 请求游客访问
    Task<GuestAccessResponse> GuestAccessRequest(GuestAccessInfo request, string client_ip);

    // 请求通过手机验证码重置密码
    Task<AccountResetPasswordResponse> ResetPasswordByPhoneRequest(AccountResetPasswordByPhoneRequest request, string client_ip);

    // 请求修改性别
    Task<AccountChangeGenderResponse> ChangeGenderRequest(AccountChangeGenderInfo request, string client_ip);

    // 请求上传头像
    Task<AccountUploadProfileImageResponse> UploadProfileImageRequest(AccountUploadProfileImageRequest request, string client_ip);

    // 请求实名认证
    Task<IdCardResponse> IdCardCheckCardAndNameRequest(CheckCardAndNameRequest request, string client_ip);

    // 请求实名认证
    Task<IdCardResult> IdCardResultRequest(IDCradResultRequest request, string client_ip);

    // 请求获取Ip所在地
    Task<SerializeObj<UCenterErrorCode, string>> GetIPAddressRequest(IPCheckRequest request, string client_ip);

    // 请求修改手机号？
    Task<UCenterErrorCode> ModifyPhoneNumber(ModifyPhoneNumberRequest request, string client_ip);

    // 请求获取指定账号关联的手机号
    Task<GetPhoneNumberResponse> GetPhoneNumber(GetPhoneNumberRequest request, string client_ip);

    // 手机验证码登录
    Task<PhoneVerificaitonCodeAccessResponse> PhoneVertificationCodeAccessRequest(PhoneVerificaitonCodeAccessInfo request, string client_ip);

    // 请求获取AppData，待Review
    Task<AppAccountDataResponse> GetAppData(Dictionary<string, string> request, string client_ip);
}