namespace DEF.CustomerSvc
{
    // 错误代码
    public enum CustomerSvcErrorCode
    {
        Error = 0,// 错误
        NoError,// 没有错误
        NoPermission,// 没有权限
        IllegalRequest,// 非法请求
    }
}