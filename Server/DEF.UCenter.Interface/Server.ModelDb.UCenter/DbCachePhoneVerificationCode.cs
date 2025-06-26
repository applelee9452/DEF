namespace DEF.UCenter;

// 手机验证码
public class CachePhoneVerificationCode : DataBase
{
    public string PhoneCode { get; set; }
    public string PhoneNumber { get; set; }
    public string VerificationCode { get; set; }
}