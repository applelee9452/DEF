namespace DEF.Gateway;

public class GatewayOptions
{
    public const string Key = "DEF.Gateway";

    public bool EnableAuth { get; set; } = true;
    public string LogFileName { get; set; } = "Gateway";
    public string TcpServer { get; set; } = "SuperSocket";// SuperSocket，DotNetty，Kcp
    public int TcpServerTimeout { get; set; } = 10;// 10秒超时
    public int TcpServerTimeoutForDebug { get; set; } = 3600;// Debug模式3600秒超时
    public int ListenPortHttp { get; set; } = 5000;
    public int ListenPortHttps { get; set; } = 5001;
    public int ListenPortTcp { get; set; } = 5002;
    public string SslFileName { get; set; } = "localhost.pfx";
    public string SslPwd { get; set; } = "123456";
    public string AppId4UCenter { get; set; } = string.Empty;
}