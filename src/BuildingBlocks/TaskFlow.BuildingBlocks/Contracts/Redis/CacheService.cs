using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;
using TaskFlow.BuildingBlocks.Interfaces;

namespace TaskFlow.BuildingBlocks.Contracts.Redis
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache cache;

        public CacheService(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await cache.GetAsync(key);
            if (value != null)
            {
                var json = Encoding.UTF8.GetString(value);
                return JsonSerializer.Deserialize<T>(json);
            }
            return default;

        }

        public async Task RemoveAsync(string key)
        {
            await cache.RemoveAsync(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (expiration is null)
            {
                expiration = TimeSpan.FromMinutes(60);
            }
            await cache.SetAsync(key, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)), new DistributedCacheEntryOptions
            {
                SlidingExpiration = expiration
            });
        }
    }
}
