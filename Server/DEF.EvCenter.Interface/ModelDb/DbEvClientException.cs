namespace DEF.EvCenter.Db;

public class EvClientException
{
    public string Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public string ClientIp { get; set; }
    public string AccId { get; set; }
    public string PlayerGuid { get; set; }
    public string Excepiton { get; set; }
    public Dictionary<string, string> DeviceInfo { get; set; }
}