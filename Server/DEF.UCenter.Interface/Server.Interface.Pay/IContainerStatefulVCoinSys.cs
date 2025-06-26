namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "VCoinSys", ContainerStateType.Stateless)]
public interface IContainerStatefulVCoinSys : IContainerRpc
{
    // 增加系统VCoin总额
    Task AddVCoinVolme(string currency, decimal volme);

    // 减少系统VCoin总额
    Task SubtractVCoinVolme(string currency, decimal volme);

    // 根据VCoin类型，查询汇率f，Gold=VCoin*f
    Task<double> QueryGoldVCoinExchangeRate(string currency);

    // 根据VCoin类型，查询汇率f，VCoin=Gold*f
    Task<double> QueryVCoinGoldExchangeRate(string currency);
}