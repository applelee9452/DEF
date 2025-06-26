//using Orleans.Concurrency;
//using System.Threading.Tasks;

//namespace DEF.Gateway;

//// 网关节点，一个网关进程对应一个网关节点
//public interface IGrainGatewayNodeStateful : IGrainWithStringKey
//{
//    // 保活
//    [OneWay]
//    Task Touch();

//    // 网关监听
//    Task SubGateway(IGrainContainerStatefulObserver sub);

//    // 网关取消监听
//    Task UnsubGateway(IGrainContainerStatefulObserver sub);

//    // 服务端主动断开连接
//    Task DisConnectSessionByOrleans(string session_guid);

//    // 通知客户端
//    Task Notify(string session_guid, ushort method_id, byte[] method_data);

//    // Session认证成功
//    Task SessionAuthed(GatewayAuthedInfo info, string extra_data);

//    // 断开Session
//    Task SessionDisConnect(string session_guid);

//    // Session请求
//    Task SessionRequest(string session_guid, ushort method_id, byte[] data);
//}