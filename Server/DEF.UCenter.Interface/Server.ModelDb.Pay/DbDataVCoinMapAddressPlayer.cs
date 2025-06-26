namespace DEF.UCenter;

// 用于根据Address，Memo查找到关联的Player
public class DataVCoinMapAddressPlayer
{
    public string Id { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    public string Address { get; set; }
    public string Memo { get; set; }
    public string PlayerGuid { get; set; }
}