using FlashMediator;
using TaskFlow.BuildingBlocks.Interfaces;

namespace TaskFlow.BuildingBlocks.Behaviors
{
    public class LimitBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ILimitedQueryable
    {
        private readonly IEnumerable<ISubscriptionLimitCheckerService> _checkers;

        public LimitBehavior(IEnumerable<ISubscriptionLimitCheckerService> checkers)
        {
            _checkers = checkers;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            var checker = _checkers
                .FirstOrDefault(c => c.LimitType == request.limitType)
                ?? throw new NotSupportedException(
                    $"{request.limitType} Limit Tipi İçin Kontrolcü bulunamadı!");

            await checker.CheckLimitAsync(request.TenantId);


            return await next();
        }
    }
}
