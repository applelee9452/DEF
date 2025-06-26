namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "Charge", ContainerStateType.Stateful)]
public interface IContainerStatefulCharge : IContainerRpc
{
    // 获取未结单订单列表
    //Task<GetUnFinishChargeListResponse> PayChargeGetUnFinishList(string client_ip);

    // 获取订单详情
    Task<PayChargeDetail> PayChargeGetDetail(string charge_id, string client_ip);

    // 创建订单
    Task<PayChargeDetail> PayChargeCreate(PayCreateChargeRequest request, string client_ip);

    // 取消订单
    Task<PayChargeInfo> PayChargeCancel(string charge_id, string client_ip);

    // Ack
    Task<PayChargeInfo> PayChargeAck(string charge_id, string client_ip);

    // 完成订单
    Task<PayChargeInfo> PayChargeSuccessComplete(string charge_id, string client_ip);

    Task PayChargeErrorClose(string charge_id, PayErrorCode pay_error_code);

    // 第三方服务器通知完成订单
    Task<string> PayChargeFinishByServer(DataPayCharge charge, string client_ip);

    // 创建admob广告 custom_data 自定义参数
    Task<string> AdCreate();

    // 客户端观看完成
    Task<bool> AdFinishedShow(string ad_guid);

    // 广告取消
    Task<bool> AdCancel(string ad_guid);

    // 广告回调 处理
    Task<bool> AdCallback(bool ret, string ad_guid, string account_id, string ad_network, string ad_unit, string transaction_id, string key_id, string reward_item, int reward_amount);

    // 穿山甲广告回调 处理
    Task<bool> GroMoreCallback(bool ret, string user_id, string trans_id, int reward_amount, string reward_name, string prime_rit, string ad_order_id);
}