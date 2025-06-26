namespace DEF.UCenter;

public class ContainerStatelessPayCenter : ContainerStateless, IContainerStatelessPayCenter
{
    public override Task OnCreate()
    {
        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    // 充值订单，请求创建
    Task<PayChargeDetail> IContainerStatelessPayCenter.PayCreateCharge(PayCreateChargeRequest request, string client_ip)
    {
        var container = GetContainerRpc<IContainerStatelessPayService>();
        return container.PayCreateCharge(request, client_ip);
    }

    // 充值订单，请求取消
    Task<PayChargeInfo> IContainerStatelessPayCenter.PayCancelCharge(string acc_id, string token, string charge_id, string client_ip)
    {
        var container = GetContainerRpc<IContainerStatelessPayService>();
        return container.PayCancelCharge(acc_id, token, charge_id, client_ip);
    }

    // Ack
    Task<PayChargeInfo> IContainerStatelessPayCenter.PayAckCharge(string acc_id, string token, string charge_id, string client_ip)
    {
        var container = GetContainerRpc<IContainerStatelessPayService>();
        return container.PayAckCharge(acc_id, token, charge_id, client_ip);
    }

    // 充值订单，请求结束
    Task<PayChargeInfo> IContainerStatelessPayCenter.PayChargeSuccessComplete(string acc_id, string token, string charge_id, string client_ip)
    {
        var container = GetContainerRpc<IContainerStatelessPayService>();
        return container.PayChargeSuccessComplete(acc_id, token, charge_id, client_ip);
    }

    // 兑换订单，请求创建
    Task<string> IContainerStatelessPayCenter.PayExchangeCreate(PayExchangeCreateRequest request, string client_ip)
    {
        var container = GetContainerRpc<IContainerStatelessPayService>();
        return container.PayExchangeCreate(request, client_ip);
    }

    // 广告，客户端请求创建
    Task<string> IContainerStatelessPayCenter.AdCreate(string acc_id)
    {
        var container = GetContainerRpc<IContainerStatelessPayService>();
        return container.AdCreate(acc_id);
    }

    // 广告，客户端观看完成
    Task<bool> IContainerStatelessPayCenter.AdFinishedShow(string acc_id, string ad_guid)
    {
        var container = GetContainerRpc<IContainerStatelessPayService>();
        return container.AdFinishedShow(acc_id, ad_guid);
    }

    // 广告，客户端取消观看广告
    Task<bool> IContainerStatelessPayCenter.AdCancel(string acc_id, string ad_guid)
    {
        var container = GetContainerRpc<IContainerStatelessPayService>();
        return container.AdCancel(acc_id, ad_guid);
    }

#if !DEF_CLIENT

    // 美达，WebHook
    Task<string> IContainerStatelessPayCenter.MeidaWebhook(IEnumerable<KeyValuePair<string, string>> map_data)
    {
        var container = GetContainerRpc<IContainerStatelessChannelMeida>();
        return container.MeidaWebhook(map_data);
    }

#endif
}