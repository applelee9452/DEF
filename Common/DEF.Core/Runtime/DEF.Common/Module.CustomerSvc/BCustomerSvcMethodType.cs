namespace DEF.CustomerSvc
{
    public enum CustomerSvcMethodType : short
    {
        // CustomerSvc
        CustomerSvcBegin = 90,

        RequestGetNewReplay = 100,// 请求获取新回复
        LoginResponse = 101,// 登录
        RequestSendTextMsg = 110,// 请求发送文字消息
        RequestSendImageMsg = 111,// 请求发送图片消息
        GetPhoneVerificationCodeResponse = 112,// 获取手机验证码

        CustomerSvcEnd = 300,
    }
}