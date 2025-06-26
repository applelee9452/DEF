//using Rabbit.Zookeeper;
//using Rabbit.Zookeeper.Implementation;
//using System;
//using System.Threading.Tasks;

//namespace DEF;

//public class ZkGateway
//{
//    IZookeeperClient Client { get; set; }
//    string NodeClusterName { get; set; }
//    string GatewayIp { get; set; }

//    string ZkConnectString { get; set; } = "127.0.0.1:2181";

//    public ZkGateway(string node_cluster_name, string gateway_ip)
//    {
//        NodeClusterName = node_cluster_name;
//        GatewayIp = gateway_ip;

//#if DEBUG
//        Client = new ZookeeperClient(new ZookeeperClientOptions(ZkConnectString)
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
//        Client = new ZookeeperClient(new ZookeeperClientOptions(ZkConnectString)
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
//    }

//    public async Task GatewayStartup()
//    {
//        // 检查并创建父节点

//        string node1 = NodeClusterName + "/" + ServiceType.Gateway.ToString();
//        string node2 = node1 + "/" + GatewayIp;

//        bool exist = await Client.ExistsAsync(node1);

//        if (!exist)
//        {
//            await Client.CreateRecursiveAsync(node1, null);
//        }

//        // 创建临时节点

//        exist = await Client.ExistsAsync(node2);

//        if (!exist)
//        {
//            await Client.CreateEphemeralAsync(node2, null);
//        }
//    }
//}