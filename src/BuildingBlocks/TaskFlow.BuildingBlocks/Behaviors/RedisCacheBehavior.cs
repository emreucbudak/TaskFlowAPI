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

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
