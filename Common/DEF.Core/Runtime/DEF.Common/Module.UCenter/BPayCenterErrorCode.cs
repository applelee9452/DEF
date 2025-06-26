namespace DEF.UCenter
{
    // 错误代码
    public enum PayErrorCode
    {
        NoError = 0,// 没有错误
        Error,// 错误
        NoPermission,// 没有权限
        UnSupported,// 不支持
        IllegalRequest,// 非法请求

        PayOrderRepeat,// 支付订单重复
        PayFeeMismatch,// 支付金币不匹配
        PayInvalidOrder,// 无效订单
        PayInvalidReceipt,// 无效票据
        PayTestOrder,// 测试订单
        PayNetError,// 网络错误
        PayUnauthorized,// 支付签名验证错误\
        PayChargeCreateFail,// 从第三方创建订单失败
    }
}