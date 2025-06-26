namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "UCenterTest", ContainerStateType.Stateless)]
public interface IContainerStatelessUCenterTest : IContainerRpc
{
    // 测试
    Task Test();

    // 批量注册一批用户，绑定已有代理Id
    Task TestCreateUsers(int count);

    // 批量生成一批支付订单
    Task TestCreatePayCharges(int paycharge_count, int account_count, DateTime dt_begin, DateTime dt_end);
}