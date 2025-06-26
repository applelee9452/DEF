using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEF;

public class ServiceDiscover
{
    public Func<string, bool, Task> ActionServiceNodeChanged { get; set; }
    public IOptions<DEFOptions> DEFOptions { get; set; }
    public IOptions<ServiceOptions> ServiceOptions { get; set; }
    public IOptions<ServiceClientOptions> ServiceClientOptions { get; set; }
    public string ClusterName { get; set; }
    public string ServiceName { get; set; }
    public string HostIpPort { get; set; }
    public string ZkConnectString { get; set; }
    ILogger Logger { get; set; }
    //ServiceDiscoverZk ServiceDiscoverZk { get; set; }

    public ServiceDiscover(
        ILogger<ServiceDiscover> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ServiceOptions> service_options,
        IOptions<ServiceClientOptions> serviceclient_options)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceOptions = service_options;
        ServiceClientOptions = serviceclient_options;
        ZkConnectString = DEFOptions.Value.ZkConnectString;
        ClusterName = "DEF.Zk";

        if (ServiceOptions.Value != null)
        {
            System.Net.IPAddress ip = Utils.GetLocalIpAddress(DEFOptions.Value.LocalIpPrefix);
            HostIpPort = string.Format("{0}:{1}", ip, ServiceOptions.Value.OrleansSiloPort);
            ServiceName = ServiceOptions.Value.ServiceName;
        }
    }

    public async Task StartAsync()
    {
        //if (ServiceDiscoverZk == null)
        //{
        //    ServiceDiscoverZk = new ServiceDiscoverZk(this, Logger);
        //    await ServiceDiscoverZk.StartAsync();
        //}

        await Task.Delay(1);
    }

    public async Task StopAsync()
    {
        //if (ServiceDiscoverZk != null)
        //{
        //    await ServiceDiscoverZk.StopAsync();
        //    ServiceDiscoverZk = null;
        //}

        await Task.Delay(1);
    }
}