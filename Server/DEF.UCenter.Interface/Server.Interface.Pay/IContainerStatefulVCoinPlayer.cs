namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "VCoinPlayer", ContainerStateType.Stateless)]
public interface IContainerStatefulVCoinPlayer : IContainerRpc
{
    // PayAzex Webhook，生成指定虚拟币的信息
    Task PayAzexWebhookGenerateAddress(string currency, string address, string memo);

    // PayAzex Webhook，提现地址有效性验证
    Task PayAzexWebhookWithdrawAddressValidation(string address, string memo, string isvalid);

    // PayAzex Webhook，提现状态改变
    Task PayAzexWebhookWithdrawStatusChange(DataVCoinTransRecord record, string withdraw_id, string status);

    // PayAzex Webhook，充值
    Task PayAzexWebhookCharge(string id, string currency, string address, string memo, string volume, string fee);
}