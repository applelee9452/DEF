

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DEF.Gateway;

public class SignalRHub : Hub
{
    ILogger<SignalRHub> Logger;
    public static ConcurrentDictionary<string, uint> MapConnection = new ConcurrentDictionary<string, uint>();// Key=SignalIR ConnectionId, Value=Orleans ConnectionId
    public static ConcurrentDictionary<uint, string> MapConnection2 = new ConcurrentDictionary<uint, string>();

    public SignalRHub(ILogger<SignalRHub> logger)
    {
        Logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        //Console.WriteLine("新建连接: " + Context.ConnectionId);
        //Console.WriteLine(Context.UserIdentifier);

        //var grain_client = OrleansClientService.Client.GetGrain<IGrainGatewayStatefullNode>(OrleansClientService.NodeGuid);
        //SessionConnectInfo sci = await grain_client.SessionConnect(Context.ConnectionId, string.Empty);

        //MapConnection.TryAdd(Context.ConnectionId, sci.session_id);
        //MapConnection2.TryAdd(sci.session_id, Context.ConnectionId);

        //await Clients.Client(Context.ConnectionId).SendAsync("Connected", Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        MapConnection.TryRemove(Context.ConnectionId, out uint connect_id);
        MapConnection2.TryRemove(connect_id, out string con_id);

        //var grain_client = OrleansClientService.Client.GetGrain<IGrainGatewayStatefullNode>(OrleansClientService.NodeGuid);
        //await grain_client.SessionDisConnect(connect_id);

        await base.OnDisconnectedAsync(exception);

        //Console.WriteLine("断开连接: " + Context.ConnectionId);

        await Clients.Client(Context.ConnectionId).SendAsync("DisConnected", Context.ConnectionId);
    }

    public Task SendMessage(int message_id, object message)
    {
        MapConnection.TryGetValue(Context.ConnectionId, out uint connect_id);

        //var jarray = (Newtonsoft.Json.Linq.JArray)message;
        var jarray = (System.Text.Json.JsonElement)message;
        var item = jarray[0];
        string str = item.ToString();

        //Console.WriteLine(str);
        Logger.LogInformation("接收到Client请求，connect_id={connect_id}，method_id={method_id}", connect_id, (ushort)message_id);

        byte[] data = System.Text.Encoding.UTF8.GetBytes(str);

        //var grain_client = OrleansClientService.Client.GetGrain<IGrainGatewayStatefullNode>(OrleansClientService.NodeGuid);
        //return grain_client.SessionRequest(connect_id, (ushort)message_id, data);

        return Task.CompletedTask;
    }
}