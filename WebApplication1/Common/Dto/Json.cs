namespace WebApplication1;

public class Json<T>
{
    public int code { get; set; }
    public string? message { get; set; }
    public T? data { get; set; }
    public long time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
}