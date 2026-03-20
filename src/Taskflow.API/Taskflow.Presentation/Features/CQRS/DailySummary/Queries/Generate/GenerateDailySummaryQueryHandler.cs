using FlashMediator;
using Microsoft.AspNetCore.Http;
using TaskFlow.BuildingBlocks.Exceptions;
using Taskflow.Presentation.Services;

namespace Taskflow.Presentation.Features.CQRS.DailySummary.Queries.Generate;

public sealed class GenerateDailySummaryQueryHandler(
    IDailySummaryService dailySummaryService,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GenerateDailySummaryQueryRequest, GenerateDailySummaryQueryResponse>
{
    public async Task<GenerateDailySummaryQueryResponse> Handle(
        GenerateDailySummaryQueryRequest request, CancellationToken cancellationToken)
    {
        var user = httpContextAccessor.HttpContext?.User
            ?? throw new AuthExceptions("No authenticated user.");

        var summary = await dailySummaryService.GenerateDailySummaryAsync(user, cancellationToken);
        return new GenerateDailySummaryQueryResponse(summary);
    }
}
