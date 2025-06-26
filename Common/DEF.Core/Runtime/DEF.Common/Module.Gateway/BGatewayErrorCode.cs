namespace DEF.Gateway
{
    // 错误代码
    public enum GatewayErrorCode
    {
        Error = 0,// 错误
        NoError,// 没有错误
        NoPermission,// 没有权限
        IllegalRequest,// 非法请求
    }
}