using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:29014");

var app = builder.Build();
app.UseWebSockets();
var connections = new List<WebSocket>();


// app.Map - 모든 HTTP 메서드에 대해 앤드포인트를 매핑합니다.
app.Map("/wss", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var name = context.Request.Query["name"];

        using var wss = await context.WebSockets.AcceptWebSocketAsync();

        connections.Add(wss);

        await Broadcast($"{name} Joined the room");
        await Broadcast($"Total connections: {connections.Count}");

        await ReceiveMessage(wss, async (result, buffer) =>
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await Broadcast($"{name}: {message}");
            }
            else if (result.MessageType == WebSocketMessageType.Close || wss.State == WebSocketState.Aborted)
            {
                connections.Remove(wss);

                await Broadcast($"{name} Left the room");
                await Broadcast($"Total connections: {connections.Count}");

                var statue = result.CloseStatus != null ? result.CloseStatus.Value : WebSocketCloseStatus.NormalClosure;
                await wss.CloseAsync(statue, result.CloseStatusDescription, CancellationToken.None);
            }
        });
    }
    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    }
});


async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
{
    var buffer = new byte[1024 * 4];
    while (socket.State == WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        handleMessage(result, buffer);
    }
}

async Task Broadcast(string message)
{
    var bytes = Encoding.UTF8.GetBytes(message);
    foreach (var socket in connections)
    {
        if (socket.State == WebSocketState.Open)
        {
            var arraySegment = new ArraySegment<byte>(bytes);
            await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}


await app.RunAsync();


/*
public enum WebSocketState
{
    None = 0,
    Connecting = 1,
    Open = 2,
    CloseSent = 3,
    CloseReceived = 4,
    Closed = 5,
    Aborted = 6
}

1. None
2. Connecting: The connection is negotiating the handshake with the remote endpoint.
3. Open: Initial state after the HTTP handshake has been completed.
4. CloseSent: Close message was sent to the client.
5. CloseReceived: Close message was received from the client.
6. Closed: Close handshake completed gracefully.
7. Aborted: The connection is aborted.

--> app.MapGet, app.MapPost, app.MapPut, app.MapDelete
- app.MapGet: GET 요청을 처리합니다.
- app.MapPost: POST 요청을 처리합니다.
- app.MapPut: PUT 요청을 처리합니다.
- app.MapDelete: DELETE 요청을 처리합니다.

// GET 요청을 처리하는 엔드포인트
app.MapGet("/api/getExample", async context =>
{
    await context.Response.WriteAsync("This is a GET request");
});

// POST 요청을 처리하는 엔드포인트
app.MapPost("/api/postExample", async context =>
{
    await context.Response.WriteAsync("This is a POST request");
});

// PUT 요청을 처리하는 엔드포인트
app.MapPut("/api/putExample", async context =>
{
    await context.Response.WriteAsync("This is a PUT request");
});

// DELETE 요청을 처리하는 엔드포인트
app.MapDelete("/api/deleteExample", async context =>
{
    await context.Response.WriteAsync("This is a DELETE request");
});

 */
