using Microsoft.Extensions.Caching.Memory;

namespace WebApplication1;

public class CachedConfigService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IMemoryCache cache;
    public CachedConfigService(IServiceScopeFactory scopeFactory,IMemoryCache cache)
    {
        this.scopeFactory = scopeFactory;
        this.cache = cache;
        using (var scope = scopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var list = db.Configs.ToList();
            foreach (var cf in list)
            {
                cache.Set(cf.key, cf.val);
            }
        }
    }

    public T Get<T>(string key)
    {

          var val = cache.GetOrCreate(key, _ =>
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    //entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    var cf = db.Configs.SingleOrDefault(e => e.key == key);
                    return cf?.val;
                }
            });

            if (val == null)
            {
                throw new Exception("Key not found");
            }
            try
            {
                return (T)Convert.ChangeType(val, typeof(T));
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException($"Cannot convert value to type {typeof(T)}");
            }

    }

    public void Set(string key, string value)
    {
        cache.Set(key, value);

    }
}