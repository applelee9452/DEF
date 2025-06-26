namespace DEF.EvCenter.Db;

public class EvClientCrashReport
{
    public string Id { get; set; }

    public DateTime Dt { get; set; }

    public string ClientIp { get; set; }

    public string AccId { get; set; }

    public string PlayerGuid { get; set; }

    public string Message { get; set; }

    public string LogType { get; set; }

    public string StackTrace { get; set; }

    public string Platform { get; set; }

    public string DeviceModel { get; set; }

    public string OS { get; set; }

    public string OperatingSystemFamily { get; set; }

    public string DeviceName { get; set; }

    public string DeviceType { get; set; }

    public string GraphicsDeviceName { get; set; }

    public string GraphicsDeviceType { get; set; }

    public string GraphicsDeviceVersion { get; set; }

    public int GraphicsMemorySize { get; set; }

    public string ProcessorType { get; set; }

    public int SystemMemorySize { get; set; }

    public int ProcessorCount { get; set; }

    public int ProcessorFrequency { get; set; }
}