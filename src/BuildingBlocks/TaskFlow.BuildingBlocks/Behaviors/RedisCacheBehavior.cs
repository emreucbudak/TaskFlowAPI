using FlashMediator;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Interfaces;

namespace TaskFlow.BuildingBlocks.Behaviors
{
    public class RedisCacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheableQuery
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<RedisCacheBehavior<TRequest, TResponse>> _logger;

        public RedisCacheBehavior(
            ICacheService cacheService,
            ILogger<RedisCacheBehavior<TRequest, TResponse>> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheKey = request.CacheKey;

            try
            {
                var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey);
                if (cachedResponse != null)
                {
                    return cachedResponse;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cache read failed for key {CacheKey}. Continuing without cache.", cacheKey);
            }

            var response = await next();

            if (response != null)
            {
                try
                {
                    await _cacheService.SetAsync(cacheKey, response, request.ExpirationTime);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Cache write failed for key {CacheKey}. Response will still be returned.", cacheKey);
                }
            }

            return response;
        }
    }
}
