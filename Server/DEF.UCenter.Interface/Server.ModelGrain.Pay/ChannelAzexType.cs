namespace DEF.UCenter;

// 提现审核结果
public enum WithdrawlValidResult
{
    None = 0,
    OverDailyLimit = 1,// 超过日限额
    LackOfBalance = 2,// 余额不足
    ManualVerifyLine = 3// 需要人工处理
}

// 提现状态
public enum CoinWithdrawlStatus
{
    None = 0,
    Started = 1,// 开始处理
    Pending = 2,// 等待人工处理
    Processing = 3,// 币种监控器正在处理
    Completed = 4,// 完成
    CreateFailed = 5,// 创建失败
    ProcessingFailed = 6,// 处理失败
    Canceled = 7,// 提现取消
    Unknown = 8// 未知状态
}

public class GetCurrencyWithdrawlInfoInput
{
    public string Currency { get; set; }
}

public class GenerateAddressInput
{
    public int MerchantId { get; set; }
    public string Currency { get; set; }
}

public class WithdrawlInput
{
    public int MerchantId { get; set; }
    public string Currency { get; set; }
    public string Address { get; set; }
    public string Memo { get; set; }
    public decimal Volume { get; set; }
}

public class WithdrawlAddressValidationInput
{
    public int MerchantId { get; set; }
    public string Currency { get; set; }
    public string Address { get; set; }
    public string Memo { get; set; } = null;
}

public class GetWithdrawInfoInput
{
    public int MerchantId { get; set; }
    public string WithdrawId { get; set; }
}

public class CurrencyLimit
{
    public decimal WithdrawlOnceMin { get; set; }// 最小提币额
    public int WithdrawlPrecision { get; set; }// 提币精度
}

public class MerchantWithdrawInfoDto
{
    public string Id { get; set; }
    public string BillId { get; set; }
    public string Currency { get; set; }// 币种
    public string Address { get; set; }// 提币地址
    public decimal Volume { get; set; }// 提币数量
    public decimal Fee { get; set; }// 手续费
    public string FeeCurrency { get; set; }// 手续费币种
    public string Memo { get; set; }// 备注码
    public string Tag { get; set; }// 提币地址名称，若该地址属于用户自己存储的地址，(在Web层赋值)
    public string TxNo { get; set; }// 交易流水号
    public WithdrawlValidResult ValidResult { get; set; }// 审核结果
    public string Status { get; set; }// 状态
    public DateTime CreatedAt { get; set; }// 创建时间
    public DateTime DoneAt { get; set; }// 处理时间
}