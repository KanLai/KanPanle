using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication1;
using WebApplication1.Attrs;

namespace SignalRWebpack.Hubs;
[AuthorizationMessage("请先登录")]
[ServiceFilter(typeof(AsyncAuthorizationFilter))]
public class ChatHub : Hub<IChatClient>
{
    public async Task NewMessage(ChatMessage chatMessage)
    {
        Console.WriteLine($"NewMessage: {chatMessage.Username} {chatMessage.Message}");
        chatMessage.Username += "says:";
        await Clients.All.ReceiveMessage("messageReceived", chatMessage);
    }
}