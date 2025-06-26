namespace DEF.UCenter;

public class VCoinItem
{
    public string Currency { get; set; }
    public string Address { get; set; }
    public string Memo { get; set; }
}

// _id=PlayerGuid
public class DataVCoinPlayer
{
    public string Id { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    public List<VCoinItem> ListVCoinItem { get; set; }
}