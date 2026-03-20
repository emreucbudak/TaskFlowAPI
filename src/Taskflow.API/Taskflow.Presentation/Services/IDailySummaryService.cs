using System.Security.Claims;

namespace Taskflow.Presentation.Services;

public interface IDailySummaryService
{
    Task<string> GenerateDailySummaryAsync(
        ClaimsPrincipal user,
        CancellationToken cancellationToken = default);
}
