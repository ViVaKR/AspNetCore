using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs;

public class ChatHub : Hub
{
    public async Task NewMessage(long userName, string message)
    => await Clients.All.SendAsync("MessageReceived", userName, message);
}
