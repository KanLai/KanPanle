using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace WebApplication1;

public class AesResolver :IFormatterResolver
{
    public IMessagePackFormatter<T>? GetFormatter<T>()
    {
        return StandardResolver.Instance.GetFormatter<T>();
    }
}