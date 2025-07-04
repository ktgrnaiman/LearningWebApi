using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Learning.Extensions;

public static class DistributedCacheExtensions
{
    public static bool TryGetValue<T>(this IDistributedCache cache, string key, out T? value)
    {
        value = default;
        var result = cache.Get(key);
        if (result is null)
            return false;
        value = JsonSerializer.Deserialize<T>(result);
        return true;
    }
    
    public static void Set<T>(this IDistributedCache cache, string key, T value, TimeSpan absoluteExpirationTimeFromNow)
    {
        cache.Set<T>(key, value, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = absoluteExpirationTimeFromNow,
        });
    }
    
    public static void Set<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));
        cache.Set(key, bytes, options);
    }
}