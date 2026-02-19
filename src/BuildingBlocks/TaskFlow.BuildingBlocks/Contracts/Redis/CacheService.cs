using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using TaskFlow.BuildingBlocks.Interfaces;

namespace TaskFlow.BuildingBlocks.Contracts.Redis
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache cache;
        private readonly ILogger<CacheService> logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            this.cache = cache;
            this.logger = logger;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var value = await cache.GetAsync(key);
                if (value != null)
                {
                    var json = Encoding.UTF8.GetString(value);
                    return JsonSerializer.Deserialize<T>(json);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis cache read failed for key {CacheKey}. Falling back to source.", key);
            }

            return default;
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis cache remove failed for key {CacheKey}.", key);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            if (expiration is null)
            {
                expiration = TimeSpan.FromMinutes(60);
            }

            try
            {
                await cache.SetAsync(key, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value)), new DistributedCacheEntryOptions
                {
                    SlidingExpiration = expiration
                });
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis cache write failed for key {CacheKey}.", key);
            }
        }
    }
}
