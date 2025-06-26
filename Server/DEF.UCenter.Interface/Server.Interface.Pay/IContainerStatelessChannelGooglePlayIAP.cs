namespace DEF.UCenter;

public class GooglePlayVerifyResponse
{
    public PayErrorCode PayErrorCode { get; set; }
    public string Transaction { get; set; }
    public string IAPProductId { get; set; }
    public string PurchaseToken { get; set; }
    public bool IsSandbox { get; set; }
}

[ContainerRpc("DEF.UCenter", "ChannelGooglePlayIAP", ContainerStateType.Stateless)]
public interface IContainerStatelessChannelGooglePlayIAP : IContainerRpc
{
    Task<GooglePlayVerifyResponse> VerifyReceipt(PayVerifyChargeRequest request, string transaction, string iap_productid);
}