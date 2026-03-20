using FlashMediator;

namespace Taskflow.Presentation.Features.CQRS.DailySummary.Queries.Generate;

public sealed record GenerateDailySummaryQueryRequest : IRequest<GenerateDailySummaryQueryResponse>;
