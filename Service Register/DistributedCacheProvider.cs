using Community.Microsoft.Extensions.Caching.PostgreSql;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Learning.ServiceRegister;

/// <summary>
/// It takes list of IDistributedCache once at construction type.
/// So make sure all implementations are created before first service provider is called.
/// </summary>
/// <param name="provider"></param>
public class DistributedCacheProvider(IServiceProvider provider)
{
    private readonly IEnumerable<IDistributedCache> _caches = provider.GetServices<IDistributedCache>();
    
    
    public IDistributedCache? Get(string key) => key switch
    {
        "Postgres" => _caches.FirstOrDefault(c => c.GetType().Name == "PostgreSqlCache"),
        "Redis" => _caches.FirstOrDefault(c => c.GetType().Name == "RedisCacheImpl"),
        _ => null
    };

    public IDistributedCache GetRequired(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (key != "InMemory" && key != "Postgres" && key != "Redis")
            throw new ArgumentOutOfRangeException(nameof(key), "Key must be the following: Postgres, Redis");
        
        var result = Get(key);
        
        if (result is null) 
            throw new InvalidOperationException("Required service was not found");
        
        return result;
    }
}