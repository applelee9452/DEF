using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Options;

namespace DEF.Gateway;

public enum TcpServerType
{
    DotNetty = 0,
    SuperSocket,
    Kcp,
    WebSocket
}

public class GatewayContext : IHostedService
{
    public static GatewayContext Instance { get; private set; }
    public IPAddress GatewayIp { get; set; }
    public string GatewayIpStr { get; set; }
    public ILogger Logger { get; private set; }
    public IOptions<DEFOptions> DEFOptions { get; private set; }
    public IOptions<ServiceOptions> ServiceOptions { get; private set; }
    public IOptions<GatewayOptions> GatewayOptions { get; private set; }
    public DbClientMongo Mongo { get; private set; }
    public ServiceNode ServiceNode { get; private set; }
    public ServiceClient ServiceClient { get; private set; }
    public GatewayService GatewayService { get; private set; }
    public TcpServerType TcpServerType { get; private set; } = TcpServerType.SuperSocket;
    public ConcurrentDictionary<string, DotNetty.Transport.Channels.IChannelHandlerContext> MapDotNettyChannelHandler { get; private set; } = new();
    public ConcurrentDictionary<string, SuperSocketChannelHandler> MapSuperSocketChannelHandler { get; private set; } = new();// Key=Tcp连接的SessionId
    public ConcurrentDictionary<string, KcpChannelHandler> MapKcpChannelHandler { get; private set; } = new();
    public ConcurrentDictionary<string, WebSocketHandler> MapWebSocketHandler { get; private set; } = new();
    public ConcurrentQueue<SendDataItem> QueKcpSendData { get; private set; } = new();
    string AssPath { get; set; }

    public GatewayContext(ILogger<GatewayContext> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ServiceOptions> service_options,
        IOptions<GatewayOptions> gateway_options,
        ServiceNode service_node,
        ServiceClient service_client,
        GatewayService gateway_service)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceOptions = service_options;
        GatewayOptions = gateway_options;
        ServiceNode = service_node;
        ServiceClient = service_client;
        GatewayService = gateway_service;
        Instance = this;

        if (GatewayOptions.Value.TcpServer == "SuperSocket")
        {
            TcpServerType = TcpServerType.SuperSocket;
        }
        else if (GatewayOptions.Value.TcpServer == "Kcp")
        {
            TcpServerType = TcpServerType.Kcp;
        }
        else if (GatewayOptions.Value.TcpServer == "DotNetty")
        {
            TcpServerType = TcpServerType.DotNetty;
        }
        else if (GatewayOptions.Value.TcpServer == "WebSocket")
        {
            TcpServerType = TcpServerType.WebSocket;
        }

        List<Assembly> list_ass = [];
        list_ass.Add(typeof(IContainerStatelessGateway).Assembly);
        list_ass.Add(typeof(UCenter.IContainerStatelessApp).Assembly);

        var listen_services = ServiceClient.ServiceClientOptions.Value.ServiceDiscoverListenServices;
        if (listen_services != null && listen_services.Count > 0)
        {
            foreach (var i in listen_services)
            {
                if (string.IsNullOrEmpty(i.AssemblyInterfacePath) || string.IsNullOrEmpty(i.AssemblyInterfaceName)) continue;

                var ass = LoadAssembly(i.AssemblyInterfacePath, i.AssemblyInterfaceName);
                list_ass.Add(ass);
            }
        }
        ServiceClient.Service.Setup(string.Empty, [.. list_ass]);

        GatewayIp = Utils.GetLocalIpAddress(DEFOptions.Value.LocalIpPrefix);
        GatewayIpStr = GatewayIp.ToString();

        Mongo = new DbClientMongo(ServiceOptions.Value.MongoDBName, ServiceOptions.Value.MongoDBConnectString);
    }

    public Task StartAsync(CancellationToken cancellation_token)
    {
        Logger.LogInformation("GatewayContext启动成功！");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellation_token)
    {
        Logger.LogInformation("GatewayContext停止成功！");

        return Task.CompletedTask;
    }

    Assembly LoadAssembly(string ass_path, string ass_name)
    {
        string p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ass_path);
        AssPath = Path.GetFullPath(p);

        var assembly_load_context = AssemblyLoadContext.Default;
        assembly_load_context.Resolving += ContextResolving;

        string fullname = Path.Combine(AssPath, ass_name + ".dll");
        fullname = Path.GetFullPath(fullname);

        var ass = assembly_load_context.LoadFromAssemblyPath(fullname);

        return ass;
    }

    Assembly ContextResolving(AssemblyLoadContext context, AssemblyName ass_name)
    {
        string expected_path = Path.Combine(AssPath, ass_name.Name + ".dll");
        if (File.Exists(expected_path))
        {
            try
            {
                using var fs = new FileStream(expected_path, FileMode.Open);

                return context.LoadFromStream(fs);
            }
            catch (Exception ex)
            {
                Logger.LogError($"加载节点{expected_path}发生异常：{ex.Message},{ex.StackTrace}");
            }
        }
        else
        {
            //Console.WriteLine($"依赖文件不存在：{expected_path}");
        }

        return null;
    }
}