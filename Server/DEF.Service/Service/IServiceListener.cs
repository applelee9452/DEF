namespace DEF;

public interface IServiceListener
{
    IService Service { get; set; }

    Task OnSessionConnectedAndAuthed(Gateway.GatewayAuthedInfo info, string extra_data);

    Task OnSessionDisConnect(string player_guid, string session_guid);

    Task OnStreamRecvedServiceNodeEvent(string id, string data);
}