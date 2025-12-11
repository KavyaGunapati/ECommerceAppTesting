using EcommerceWebAPI.Interfaces.IManager;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace EcommerceWebAPI.Managers
{

    public class InMermoryCache : IInMemoryCache
    {
        private readonly IMemoryCache _cache;
        public InMermoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }
        public async Task<T?> Get<T>(string key)
        {
            _cache.TryGetValue(key,out T? value);
            return value;

        }
        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public async Task Set<T>(string key, T value, TimeSpan? duration = null)
        {
            if (duration.HasValue)
            {
                _cache.Set(key, value, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = duration.Value
                });
            }
            else
            {
                _cache.Set(key, value);
            }
        }

        public bool TryGet<T>(string key, out T value)
        {
            
        }
    }
}
