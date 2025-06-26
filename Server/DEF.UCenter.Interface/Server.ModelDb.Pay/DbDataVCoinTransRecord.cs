namespace DEF.UCenter;

// VCoin充值和提现记录
public class DataVCoinTransRecord
{
    public string Id { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    public string PlayerGuid { get; set; }
    public bool ChargeOrWithdraw { get; set; }// true=Charge
    public string Currency { get; set; }// 币种
    public string Address { get; set; }
    public string Memo { get; set; }
    public decimal Volume { get; set; }// 数量
    public decimal Fee { get; set; }// 手续费
    public string FeeCurrency { get; set; }// 手续费币种
    public string Tag { get; set; }// 提币地址名称，若该地址属于用户自己存储的地址，(在Web层赋值)
    public string TxNo { get; set; }// 交易流水号
    //public WithdrawlValidResult ValidResult { get; set; }// 审核结果
    public string Status { get; set; }// 状态
    public string WithdrawId { get; set; }
    public DateTime CreatedAt { get; set; }// 创建时间
    public DateTime DoneAt { get; set; }// 处理时间
}