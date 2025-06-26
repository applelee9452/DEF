namespace DEF.UCenter;

public class AppleStoreVerifyResponse
{
    public PayErrorCode PayErrorCode { get; set; }
    public string Transaction { get; set; }
    public string IAPProductId { get; set; }
    public bool IsSandbox { get; set; }
}

[ContainerRpc("DEF.UCenter", "ChannelAppleStoreIAP", ContainerStateType.Stateless)]
public interface IContainerStatelessChannelAppleStoreIAP : IContainerRpc
{
    Task<AppleStoreVerifyResponse> VerifyReceipt(PayVerifyChargeRequest request, string transaction, string iap_productid);
}