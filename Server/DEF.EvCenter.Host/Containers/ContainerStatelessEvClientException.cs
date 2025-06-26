using DEF.EvCenter.Db;
using Microsoft.Extensions.Logging;

namespace DEF.EvCenter;

public class ContainerStatelessEvClientException : ContainerStateless, IContainerStatelessEvClientException
{
    DbClientMongo Db;

    public override Task OnCreate()
    {
        Db = EvCenterContext.Instance.Mongo;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    Task IContainerStatelessEvClientException.ClientCrashReport(CrashReportInfo info, string client_ip)
    {
        Logger.LogDebug($"ContainerStatelessEvClientException.ClientException()");

        if (info == null)
        {
            return Task.CompletedTask;
        }

        EvClientCrashReport ev = new()
        {
            Id = Guid.NewGuid().ToString(),
            Dt = DateTime.UtcNow,
            ClientIp = client_ip,
            AccId = string.Empty,
            PlayerGuid = info.PlayerGuid,
            Message = info.Message,
            LogType = info.LogType,
            StackTrace = info.StackTrace,
            Platform = info.Platform,
            DeviceModel = info.DeviceModel,
            OS = info.OS,
            OperatingSystemFamily = info.OperatingSystemFamily,
            DeviceName = info.DeviceName,
            DeviceType = info.DeviceType,
            GraphicsDeviceName = info.GraphicsDeviceName,
            GraphicsDeviceType = info.GraphicsDeviceType,
            GraphicsDeviceVersion = info.GraphicsDeviceVersion,
            GraphicsMemorySize = info.GraphicsMemorySize,
            ProcessorType = info.ProcessorType,
            SystemMemorySize = info.SystemMemorySize,
            ProcessorCount = info.ProcessorCount,
            ProcessorFrequency = info.ProcessorFrequency
        };

        return Db.InsertAsync(typeof(EvClientCrashReport).Name, ev);
    }
}