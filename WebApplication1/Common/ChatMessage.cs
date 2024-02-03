using MessagePack;

namespace WebApplication1;
[MessagePackObject]
public class ChatMessage
{
    public static ChatMessage Instance()
    {
        return new ChatMessage();
    }

    [Key("username")]
    public string Username { get; set; } = string.Empty;
    [Key("message")]
    public string Message { get; set; } = string.Empty;
}