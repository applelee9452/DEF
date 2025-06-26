namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "Exchange", ContainerStateType.Stateful)]
public interface IContainerStatefulExchange : IContainerRpc
{
    // 获取未结单订单列表
    Task<string> PayExchangeGetUnFinishList(string client_ip);

    // 获取订单详情
    Task<string> PayExchangeGetDetail(string exchange_id, string client_ip);

    // 创建订单
    Task<string> PayExchangeCreate(PayExchangeCreateRequest request, string client_ip);

    //// 取消订单
    //Task<string> PayExchangeCancel(string exchange_id, string client_ip);

    //// 验证订单
    //Task<string> PayExchangeVerify(PayVerifyChargeRequest request, string client_ip);

    //// 结束订单
    //Task<string> PayExchangeFinish(string exchange_id, string client_ip);

    // 第三方服务器通知完成订单
    Task<string> PayExchangeFinishByServer(DataPayExchange exchange, string client_ip);
}