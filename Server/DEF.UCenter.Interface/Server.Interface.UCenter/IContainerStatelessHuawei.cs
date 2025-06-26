namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "Huawei", ContainerStateType.Stateless)]
public interface IContainerStatelessHuawei : IContainerRpc
{
    // 查询Ip所在地
    Task QueryIpAddress(string ip);

    // 手机三要素实名认证
    Task QueryMobileThree(string id_card, string name, string mobile);
}