using Microsoft.AspNetCore.Mvc;

namespace DEF.Gateway;

[ApiController]
public class WebSocketController : ControllerBase
{
    ILogger<WebSocketController> Logger { get; set; }
    ServiceClient ServiceClient { get; set; }

    public WebSocketController(ILogger<WebSocketController> logger, ServiceClient service_client)
    {
        Logger = logger;
        ServiceClient = service_client;
    }

    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var ws = await HttpContext.WebSockets.AcceptWebSocketAsync(new WebSocketAcceptContext { DangerousEnableCompression = false });

            WebSocketHandler handler = new(Logger, ws, ServiceClient, HttpContext.Connection.RemoteIpAddress, HttpContext.Connection.Id);

            try
            {
                await handler.SendSessionId();

                var recv_result = await ws.ReceiveAsync(new ArraySegment<byte>(handler.RecvBuffer), CancellationToken.None);

                while (recv_result.CloseStatus == null || !recv_result.CloseStatus.HasValue)
                {
                    await handler.OnRecvPackage(handler.RecvBuffer, recv_result.Count);

                    recv_result = await ws.ReceiveAsync(new ArraySegment<byte>(handler.RecvBuffer), CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }

            await handler.CloseAsync();
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    //static async Task Echo(WebSocket web_socket)
    //{
    //    var buffer = new byte[1024 * 4];
    //    var receiveResult = await web_socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    //    while (!receiveResult.CloseStatus.HasValue)
    //    {
    //        await web_socket.SendAsync(new ArraySegment<byte>(buffer, 0, receiveResult.Count),
    //            receiveResult.MessageType, receiveResult.EndOfMessage, CancellationToken.None);

    //        receiveResult = await web_socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    //    }

    //    await web_socket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
    //}
}