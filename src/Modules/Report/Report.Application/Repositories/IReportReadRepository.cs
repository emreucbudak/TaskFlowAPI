using Microsoft.EntityFrameworkCore.Query;
using TaskFlow.BuildingBlocks.Common;

namespace Report.Application.Repositories
{
    public interface IReportReadRepository
    {
        Task<Domain.Entities.Report?> GetByIdAsync(bool trackChanges, Guid id, Func<IQueryable<Domain.Entities.Report>, IIncludableQueryable<Domain.Entities.Report, object>>? inc = null);
        Task<PagedResult<Domain.Entities.Report>> GetAllAsync(
            int pageSize,
            int page = 1,
            bool trackChanges = false,
            IReadOnlyCollection<Guid>? reportingUserIds = null,
            Func<IQueryable<Domain.Entities.Report>, IIncludableQueryable<Domain.Entities.Report, object>>? inc = null);
        Task<PagedResult<Domain.Entities.Report>> GetByDepartmentAsync(Guid departmentId,
            int pageSize,
            int page = 1,
            bool trackChanges = false,
            Func<IQueryable<Domain.Entities.Report>, IIncludableQueryable<Domain.Entities.Report, object>>? inc = null);
    }
}
