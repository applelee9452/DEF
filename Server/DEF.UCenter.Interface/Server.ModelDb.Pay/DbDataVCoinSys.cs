namespace DEF.UCenter;

public class VCoinSysItem
{
    public string Currency { get; set; }
    public decimal Volme { get; set; }// 该币种的账户余额
}

// VCoin系统信息
public class DataVCoinSys
{
    public string _id { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    public List<VCoinSysItem> ListVCoinItem { get; set; }// 各个币种的总额
}