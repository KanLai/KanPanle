namespace WebApplication1;

public interface IChatClient
{
    Task ReceiveMessage(string message, ChatMessage chatMessage);
}