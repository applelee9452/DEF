using System.Threading.Tasks;

namespace DEF.UCenter
{
    [ContainerRpc("DEF.UCenter", "UCenter", ContainerStateType.Stateless)]
    public interface IContainerStatelessUCenter : IContainerRpc
    {
        // 请求登录
        Task<AccountLoginResponse> LoginRequest(AccountLoginInfo request, string client_ip);

        // 请求获取手机验证码
        Task<UCenterErrorCode> GetPhoneVerificationCodeRequest(GetPhoneVerificationCodeRequest request, string client_ip);

        // 请求注册
        Task<AccountRegisterResponse> RegisterRequest(AccountRegisterRequestInfo request, ulong agent_id, string client_ip);

        // 请问微信自动登录
        Task<AccountLoginResponse> WechatAutoLoginRequest(AccountWechatAutoLoginRequest request, string client_ip);

        // 请求微信登录
        Task<AccountLoginResponse> WechatLoginRequest(AccountWechatOAuthInfo request, string client_ip);

        // 请求微信公众号登录
        Task<AccountLoginResponse> WechatMPLoginRequest(AccountWechatOAuthInfo request, string client_ip);

        // 请求Taptap登录
        Task<AccountLoginResponse> TaptapLoginRequest(AccountTaptapLoginRequest request, string client_ip);

        // 请求Facebook登录
        Task<AccountLoginResponse> FacebookLoginRequest(AccountFacebookOAuthInfo request, string client_ip);

        // 请求谷歌登录
        Task<AccountLoginResponse> GoogleLoginRequest(AccountGoogleOAuthInfo request, string client_ip);

        // 请求Enjoy登录
        Task<AccountLoginResponse> EnjoyLoginRequest(EnjoyLoginRequest request, string client_ip);

        // 请求游客访问
        Task<GuestAccessResponse> GuestAccessRequest(GuestAccessInfo request, string client_ip);

        // 手机验证码登陆
        Task<PhoneVerificaitonCodeAccessResponse> PhoneVerificationCodeAccessRequest(PhoneVerificaitonCodeAccessInfo request, string client_ip);

        // 请求通过手机验证码重置密码
        Task<AccountResetPasswordResponse> ResetPasswordByPhoneRequest(AccountResetPasswordByPhoneRequest request, string client_ip);

        // 请求修改性别
        Task<AccountChangeGenderResponse> ChangeGenderRequest(AccountChangeGenderInfo request, string client_ip);

        // 请求上传头像
        Task<AccountUploadProfileImageResponse> UploadProfileImageRequest(AccountUploadProfileImageRequest request, string client_ip);

        // 请求身份证实名认证
        Task<IdCardResponse> IdCardCheckCardAndNameRequest(CheckCardAndNameRequest request, string client_ip);

        // 请求身份证实名认证
        Task<IdCardResult> IdCardResultRequest(IDCradResultRequest request, string client_ip);

        // 请求获取Ip所在地
        Task<SerializeObj<UCenterErrorCode, string>> GetIPAddressRequest(IPCheckRequest request, string client_ip);

        // 请求修改账号对应的手机号码
        Task<UCenterErrorCode> ModifyPhoneNumber(ModifyPhoneNumberRequest request, string client_ip);

        // 请求获取账号对应的手机号码
        Task<GetPhoneNumberResponse> GetPhoneNumber(GetPhoneNumberRequest request, string client_ip);

        // 请求创建代理账号
        Task<ulong> RequestCreateAgent(string user_name, ulong parent_agent_id);

        // 请求删除代理账号
        Task RequestDeleteAgent(ulong agent_id);
    }
}