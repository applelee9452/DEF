namespace DEF.UCenter;

public class DataDeviceGuest : DataBase
{
    public string AccountId { get; set; }
    public string AppId { get; set; }
    public DeviceInfo Device { get; set; }
}