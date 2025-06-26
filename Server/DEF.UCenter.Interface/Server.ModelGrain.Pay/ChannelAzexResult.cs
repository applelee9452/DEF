namespace DEF.UCenter;

public struct VoidResult<TE> //: IErrorResult<TE> where TE : IError
{
    public bool IsOk { get; set; }// 操作成功与否
    public TE Err { get; set; }// 错误信息
}

public struct Result<T, TE> //: IErrorResult<TE> where TE : IError
{
    public bool IsOk { get; set; }// 操作成功与否
    public T Value { get; set; }// 操作结果
    public TE Err { get; set; }// 错误信息
}

public struct Error //: IVoidError
{
    public int Code { get; set; }// 错误代码
    public string Message { get; set; }// 错误信息（已根据语言翻译）
}