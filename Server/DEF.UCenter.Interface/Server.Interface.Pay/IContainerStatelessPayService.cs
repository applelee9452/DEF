namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "PayService", ContainerStateType.Stateless)]
public interface IContainerStatelessPayService : IContainerRpc
{
    // 充值订单，请求获取未结单列表
    //Task<GetUnFinishChargeListResponse> PayGetUnFinishChargeList(string acc_id, string token, string client_ip);

    // 充值订单，请求获取详情
    Task<PayChargeDetail> PayGetChargeDetail(string acc_id, string token, string charge_id, string client_ip);

    // 充值订单，请求创建
    Task<PayChargeDetail> PayCreateCharge(PayCreateChargeRequest request, string client_ip);

    // 充值订单，请求取消
    Task<PayChargeInfo> PayCancelCharge(string acc_id, string token, string charge_id, string client_ip);

    // Ack
    Task<PayChargeInfo> PayAckCharge(string acc_id, string token, string charge_id, string client_ip);

    // 充值订单，请求验证
    //Task<PayChargeInfo> PayVerifyCharge(PayVerifyChargeRequest request, string client_ip);

    // 充值订单，请求结束
    Task<PayChargeInfo> PayChargeSuccessComplete(string acc_id, string token, string charge_id, string client_ip);

    // 兑换订单，请求创建
    Task<string> PayExchangeCreate(PayExchangeCreateRequest request, string client_ip);

    // 第三方服务器通知完成充值订单
    Task<string> PayChargeFinishByServer(string data, string client_ip);

    // 第三方服务器通知完成充值订单
    Task<string> PayChargeFinishByServer(string charge_id, int amount, string client_ip, string ret_value);

    // 广告，客户端请求创建
    Task<string> AdCreate(string acc_id);

    // 广告，客户端观看完成
    Task<bool> AdFinishedShow(string acc_id, string ad_guid);

    // 广告，客户端取消观看广告
    Task<bool> AdCancel(string acc_id, string ad_guid);

    // 广告，Admob回调
    Task<string> AdmobWebhookRequest(string ad_json);

    // 广告，GroMore回调
    Task<string> GroMoreWebhookRequest(string user_id, string trans_id, int reward_amount, string reward_name, string sign, string prime_rit, string ad_order_id);
}