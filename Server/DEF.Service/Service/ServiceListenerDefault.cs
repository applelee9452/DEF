namespace DEF;

public class ServiceListenerDefault : IServiceListener
{
    public IService Service { get; set; }

    public Task OnSessionConnectedAndAuthed(Gateway.GatewayAuthedInfo info, string extra_data)
    {
        return Task.CompletedTask;
    }

    public Task OnSessionDisConnect(string player_guid, string session_guid)
    {
        return Task.CompletedTask;
    }

    public Task OnStreamRecvedServiceNodeEvent(string id, string data)
    {
        //Console.WriteLine($"ServiceListenerDefault.OnStreamRecvedServiceNodeEvent() Id={id} Data={data}");

        return Task.CompletedTask;
    }
}