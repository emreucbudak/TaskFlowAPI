namespace Taskflow.Presentation.Services;

public interface IDailySummaryService
{
    Task<string> GenerateDailySummaryAsync(
        Guid userId,
        Guid companyId,
        bool isDepartmentLeader,
        Guid? departmentId,
        CancellationToken cancellationToken = default);
}
