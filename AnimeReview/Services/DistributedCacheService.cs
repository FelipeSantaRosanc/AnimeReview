using System.Text.Json;
using AnimeReview.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace AnimeReview.Services
{
    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public DistributedCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var data = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(data))
                return default;

            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
            };

            var data = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, data, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            // Em produção com Redis, usar StackExchange.Redis para pattern matching
            // Com MemoryCache, não há suporte nativo para pattern removal
            await Task.CompletedTask;
        }

    }
}
