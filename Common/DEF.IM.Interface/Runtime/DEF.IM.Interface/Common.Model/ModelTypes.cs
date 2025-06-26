namespace DEF.IM
{
    // IM相关错误类型枚举
    public enum IMResult
    {
        Error = 0,// 错误
        Success,// 成功
        NoPermission,// 没有权限
        IllegalRequest,// 非法请求
        ParamError,// 参数错误
    }

    public enum IMSubPubChannel
    {
        IMMail
    }

    public enum IMSubPubEventType
    {
        GetMailAttachment
    }
}