namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "ChannelAzexHelper", ContainerStateType.Stateless)]
public interface IContainerStatelessChannelAzexHelper : IContainerRpc
{
    Task AddPlayerGuid4GenerateAddress(string player_guid);

    Task<string> GetThenRemovePlayerGuid4GenerateAddress();

    Task<bool> IsExistPlayerGuid4GenerateAddress(string player_guid);

    Task AddPlayerGuid4WithdrawAddressValid(string address, string memo, string player_guid);

    Task<string> GetThenRemovePlayerGuid4WithdrawAddressValid(string address, string memo);
}