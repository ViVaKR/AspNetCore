﻿using System.Text;
using System.Net.WebSockets;

var wss = new ClientWebSocket();

string? name = "Client";

while (true)
{
    Console.WriteLine("Enter your name:");
    name = Console.ReadLine();
    if (!string.IsNullOrEmpty(name))
    {
        break;
    }
}

Console.WriteLine($"Connection to server...");
await wss.ConnectAsync(new Uri($"wss://localhost:29014/wss?name={name}"), CancellationToken.None);
Console.WriteLine("Connected");

var sendTask = Task.Run(async () =>
{
    while (true)
    {
        Console.WriteLine("Enter message:");
        var message = Console.ReadLine();
        if (string.IsNullOrEmpty(message) || message.Trim().ToLower().Equals("exit"))
        {
            if (wss.State == WebSocketState.Open)
                await wss.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);
            break;
        }
        var bytes = Encoding.UTF8.GetBytes(message);

        await wss.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }
});

var receiveTask = Task.Run(async () =>
{
    var buffer = new byte[1024];
    while (true)
    {
        var result = await wss.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            Console.WriteLine("Received close message");
            break;
        }
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine($"Received: {message}");
    }
});
await Task.WhenAny(sendTask, receiveTask);

if (wss.State == WebSocketState.Open)
{
    await wss.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);
}
await Task.WhenAll(sendTask, receiveTask);
