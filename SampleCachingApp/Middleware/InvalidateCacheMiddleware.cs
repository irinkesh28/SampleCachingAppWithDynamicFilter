using Microsoft.Extensions.Caching.Memory;
using SampleCachingApp.Helpers;

namespace SampleCachingApp.Middleware
{
    public class InvalidateCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public InvalidateCacheMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);
            var cachedKeys = GlobalCacheService.globalCacheService.Keys;
        }
    }
}
