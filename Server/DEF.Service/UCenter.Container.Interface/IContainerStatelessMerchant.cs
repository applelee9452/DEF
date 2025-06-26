namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "Merchant", ContainerStateType.Stateless)]
public interface IContainerStatelessMerchant : IContainerRpc
{
    // 获取商户指定日期推广的用户总数
    Task<int> MerchantObtainUserCount(string auth, DateTime begin_time, DateTime end_time);

    // 获取商户指定日期推广的用户列表，分页的，每页最多100条，需要指定页码
    Task<List<MerchantAccount>> MerchantObtainUserList(string auth, int page_index, DateTime begin_time, DateTime end_time);

    // 获取商户的一级代理商的总数
    Task<int> MerchantGetTopUserCount(string auth, DateTime begin_time, DateTime end_time);

    // 获取商户指定日期推广的一级代理商列表，分页的，每页最多100条，需要指定页码
    Task<List<MerchantAccount>> MerchantGetTopUserList(string auth, int page_index, DateTime begin_time, DateTime end_time);

    // 获取商户的指定日期的指定代理商的直接用户的总数
    Task<int> MerchantGetChildrenCount(string auth, string agent_id, DateTime begin_time, DateTime end_time);

    // 获取商户指定日期的指定代理商的直接用户总数，分页的，每页最多100条，需要指定页码
    Task<List<MerchantAccount>> MerchantGetChildrenList(string auth, string agent_id, int page_index, DateTime begin_time, DateTime end_time);

    // 获取商户指定日期的所有用户的充值笔数
    Task<int> MerchantGetUserRechargeCount(string auth, DateTime begin_time, DateTime end_time);

    // 获取商户指定日期的所有用户的充值列表，分页的，每页最多100条，需要指定页码
    Task<List<MerchantAccountRecharge>> MerchantGetUserRechargeList(string auth, int page_index, DateTime begin_time, DateTime end_time);
}