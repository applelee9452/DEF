using Microsoft.Extensions.Logging;
//using Rabbit.Zookeeper;
//using Rabbit.Zookeeper.Implementation;

namespace DEF;

public class ServiceDiscoverZk
{
    ILogger Logger { get; set; }
    //IZookeeperClient Client { get; set; }
    //ServiceDiscoverZkWatcher ZkWatcher { get; set; }
    ServiceDiscover ServiceDiscover { get; set; }
    string FullName { get; set; } = string.Empty;

    public ServiceDiscoverZk(ServiceDiscover service_discover, ILogger logger)
    {
        ServiceDiscover = service_discover;
        Logger = logger;

        if (!string.IsNullOrEmpty(ServiceDiscover.ClusterName)
            && !string.IsNullOrEmpty(ServiceDiscover.ServiceName)
            && !string.IsNullOrEmpty(ServiceDiscover.HostIpPort))
        {
            string node1 = ServiceDiscover.ClusterName + "/" + ServiceDiscover.ServiceName;
            string node2 = node1 + "/" + ServiceDiscover.HostIpPort;
            FullName = node2;
        }

        //#if DEBUG
        //        Client = new ZookeeperClient(new ZookeeperClientOptions(ServiceDiscover.ZkConnectString)
        //        {
        //            BasePath = "/",
        //            ConnectionTimeout = TimeSpan.FromSeconds(3),
        //            OperatingTimeout = TimeSpan.FromSeconds(3000),
        //            SessionTimeout = TimeSpan.FromSeconds(3000),
        //            ReadOnly = false,
        //            SessionId = 0,
        //            SessionPasswd = null,
        //            EnableEphemeralNodeRestore = true
        //        });
        //#else
        //        Client = new ZookeeperClient(new ZookeeperClientOptions(ServiceDiscover.ZkConnectString)
        //        {
        //            BasePath = "/",
        //            ConnectionTimeout = TimeSpan.FromSeconds(3),
        //            OperatingTimeout = TimeSpan.FromSeconds(3),
        //            SessionTimeout = TimeSpan.FromSeconds(5),
        //            ReadOnly = false,
        //            SessionId = 0,
        //            SessionPasswd = null,
        //            EnableEphemeralNodeRestore = true
        //        });
        //#endif

        //        ZkWatcher = new ServiceDiscoverZkWatcher(ServiceDiscover, Logger, Client);
    }

    public async Task StartAsync()
    {
        await Task.Delay(1);

        //if (!string.IsNullOrEmpty(FullName))
        //{
        //    // 检查并创建父节点

        //    string node1 = ServiceDiscover.ClusterName + "/" + ServiceDiscover.ServiceName;
        //    bool exist = await Client.ExistsAsync(node1);
        //    if (!exist)
        //    {
        //        await Client.CreateRecursiveAsync(node1, null);
        //    }

        //    // 创建临时节点

        //    exist = await Client.ExistsAsync(FullName);
        //    if (!exist)
        //    {
        //        await Client.CreateEphemeralAsync(FullName, null);
        //    }
        //}

        //await ZkWatcher.StartAsync();
    }

    public async Task StopAsync()
    {
        await Task.Delay(1);

        //await ZkWatcher.StopAsync();

        //if (!string.IsNullOrEmpty(FullName))
        //{
        //    bool exist = await Client.ExistsAsync(FullName);
        //    if (exist)
        //    {
        //        await Client.DeleteAsync(FullName);
        //    }

        //    var l = await GetAllServiceNode();
        //    if (l == null || l.Count == 0)
        //    {
        //        string node1 = ServiceDiscover.ClusterName + "/" + ServiceDiscover.ServiceName;
        //        await Client.DeleteAsync(node1);
        //    }

        //    var l2 = await GetAllServiceType();
        //    if (l2 == null || l2.Count == 0)
        //    {
        //        await Client.DeleteAsync(ServiceDiscover.ClusterName);
        //    }
        //}
    }

    async Task<List<string>> GetAllServiceNode()
    {
        await Task.Delay(1);

        //IEnumerable<string> children = null;

        //try
        //{
        //    children = await Client.GetChildrenAsync(string.Format("{0}/{1}",
        //        ServiceDiscover.ClusterName, ServiceDiscover.ServiceName));
        //}
        //catch (Exception)
        //{
        //}

        List<string> list_silo = new();

        //if (children != null && children.Count() > 0)
        //{
        //    list_silo.AddRange(children);
        //}

        return list_silo;
    }

    async Task<List<string>> GetAllServiceType()
    {
        await Task.Delay(1);

        //IEnumerable<string> children = null;

        //try
        //{
        //    children = await Client.GetChildrenAsync(ServiceDiscover.ClusterName);
        //}
        //catch (Exception)
        //{
        //}

        List<string> list = new();

        //if (children != null && children.Count() > 0)
        //{
        //    list.AddRange(children);
        //}

        return list;
    }
}
