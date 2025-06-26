namespace DEF.UCenter;

public class WindowsStoreVerifyResponse
{
    public PayErrorCode PayErrorCode { get; set; }
    public string Transaction { get; set; }
    public string IAPProductId { get; set; }
    public string PurchaseToken { get; set; }
    public bool IsSandbox { get; set; }
}

[ContainerRpc("DEF.UCenter", "ChannelWindowsStoreIAP", ContainerStateType.Stateless)]
public interface IContainerStatelessChannelWindowsStoreIAP : IContainerRpc
{
    Task<WindowsStoreVerifyResponse> VerifyReceipt(PayVerifyChargeRequest request, string transaction, string iap_productid);
}