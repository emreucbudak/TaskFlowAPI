using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace TaskFlow.BuildingBlocks.Behaviors
{
    public class RedisCacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheableQuery
    {
        private readonly ICacheService _cacheService;
        public RedisCacheBehavior(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
                var cacheKey = request.CacheKey;
                var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey);
                if (cachedResponse != null)
                {
                    return cachedResponse;
                }
                var response = await next();
                if (response != null)
                {
                    await _cacheService.SetAsync(cacheKey, response, request.ExpirationTime);
                }

                return response;
            

        }
    }
}
