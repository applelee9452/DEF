namespace DEF.UCenter
{
    public enum UCenterErrorCode
    {
        Error = 0,
        NoError = 1,

        // Error sends http request on client side
        ClientError = 2,

        // BadRequest - 400
        InvalidAccountName = 400001,
        InvalidAccountPassword = 400002,
        InvalidAccountEmail = 400003,
        InvalidAccountPhone = 400004,
        InvalidIp = 400005,
        InvalidParam = 400006,
        DeviceInfoNull = 400010,
        DeviceIdNull = 400011,
        AppIdNull = 400012,
        TokenOutdated = 400013,// Token过期

        // Unauthorized - 401
        AccountPasswordUnauthorized = 401001,
        AccountTokenUnauthorized = 401002,
        AppTokenUnauthorized = 401003,
        AccountDisabled = 401004,
        AccountOAuthTokenUnauthorized = 401005,
        PhoneVerificationCodeError = 401006,// 手机验证码错误
        PayUnauthorized = 401007,// 支付签名验证错误

        // NotFound - 404
        AccountNotExist = 404001,
        AppNotExists = 404002,
        OrderNotExists = 404003,

        // Conflict - 409
        AccountNameAlreadyExist = 409001,
        AppNameAlreadyExist = 409002,
        OrangeAccountCanntAttachOrange = 409003,// Orange登陆的账号无需再次绑定微信
        AttachOrangeExists = 409004,// 该账号已经绑定微信，无法重复绑定
        AttachOrangeAttachCountMax = 409005,// 该微信绑定账号次数已经达到上限，无法绑定新账号

        // InternalServerError - 500
        InternalDatabaseError = 500001,
        InternalHttpServerError = 500002,

        // ServiceUnavailable - 503
        ServiceUnavailable = 503001,

        PayOrderRepeat = 600001,// 支付订单重复
        PayFeeMismatch = 600002,// 支付金币不匹配
        PayInvalidOrder = 600003,// 无效订单
        PayInvalidReceipt = 600004,// 无效票据
        PayTestOrder = 600005,// 测试订单
        PayNetError = 600006,// 网络错误

        PayIAPOrderRepeat = 600101,// 支付订单重复
        PayIAPFeeMismatch = 600102,// 支付金币不匹配
        PayIAPInvalidOrder = 600103,// 无效订单
        PayIAPInvalidReceipt = 600104,// 无效票据
        PayIAPTestOrder = 600105,// 测试订单
        PayIAPNetError = 600106,// 网络错误
    }
}