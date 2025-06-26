
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.UCenter
{
    [ContainerRpc("DEF.UCenter", "PayCenter", ContainerStateType.Stateless)]
    public interface IContainerStatelessPayCenter : IContainerRpc
    {
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

        // 广告，客户端请求创建
        Task<string> AdCreate(string acc_id);

        // 广告，客户端观看完成
        Task<bool> AdFinishedShow(string acc_id, string ad_guid);

        // 广告，客户端取消观看广告
        Task<bool> AdCancel(string acc_id, string ad_guid);

#if !DEF_CLIENT
        // 美达，WebHook
        Task<string> MeidaWebhook(IEnumerable<KeyValuePair<string, string>> map_data);
#endif
    }
}