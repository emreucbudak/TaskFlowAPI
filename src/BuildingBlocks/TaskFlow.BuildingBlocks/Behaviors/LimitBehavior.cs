using FlashMediator;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Interfaces;

namespace TaskFlow.BuildingBlocks.Behaviors
{
    public class LimitBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ILimitedQueryable
    {
        private readonly IEnumerable<ISubscriptionLimitCheckerService> _checkers;
        private readonly ILogger<LimitBehavior<TRequest, TResponse>> _logger;

        public LimitBehavior(
            IEnumerable<ISubscriptionLimitCheckerService> checkers,
            ILogger<LimitBehavior<TRequest, TResponse>> logger)
        {
            _checkers = checkers;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken ct)
        {
            var checker = _checkers.FirstOrDefault(c => c.LimitType == request.limitType);

            if (checker == null)
            {
                _logger.LogError("No limit checker found for limit type {LimitType}", request.limitType);
                throw new NotSupportedException($"{request.limitType} Limit Tipi Icin Kontrolcu bulunamadi!");
            }

            await checker.CheckLimitAsync(request.TenantId);

            try
            {
                return await next();
            }
            catch
            {
                try
                {
                    await checker.ReleaseLimitAsync(request.TenantId);
                    _logger.LogInformation(
                        "Limit released after failure. TenantId: {TenantId}, LimitType: {LimitType}",
                        request.TenantId,
                        request.limitType);
                }
                catch (Exception releaseException)
                {
                    _logger.LogError(
                        releaseException,
                        "Failed to release limit after failure. TenantId: {TenantId}, LimitType: {LimitType}",
                        request.TenantId,
                        request.limitType);
                }

                throw;
            }
        }
    }
}
