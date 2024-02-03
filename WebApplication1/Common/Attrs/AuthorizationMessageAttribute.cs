namespace WebApplication1.Attrs;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizationMessageAttribute(string message) : Attribute
{
    public string Message { get; } = message;
}