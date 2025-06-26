#if !DEF_CLIENT

using System.Threading.Tasks;

namespace DEF.IM;

public class IMServiceListener : IServiceListener
{
    public IService Service { get; set; }

    public Task OnSessionConnectedAndAuthed(Gateway.GatewayAuthedInfo info, string extra_data)
    {
        var c = Service.GetContainerRpc<IContainerStatefulIMPlayer>(info.PlayerGuid);
        return c.ClientAttached(info, extra_data);
    }

    public Task OnSessionDisConnect(string player_guid, string session_guid)
    {
        var c = Service.GetContainerRpc<IContainerStatefulIMPlayer>(player_guid);
        return c.ClientDeattached(session_guid);
    }

    public Task OnStreamRecvedServiceNodeEvent(string id, string data)
    {
        //Console.WriteLine($"IM：Id={id} Data={data}");

        return Task.CompletedTask;
    }
}

#endif