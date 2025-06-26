namespace DEF.CCenter
{
    // 错误代码
    public enum CCenterErrorCode
    {
        Error = 0,// 错误
        NoError,// 没有错误
        NoPermission,// 没有权限
        IllegalRequest,// 非法请求
        NoFoundEnvironment,// 没有发现这个环境的版本
    }
}