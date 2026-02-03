using TaskFlow.BuildingBlocks.Common;

namespace Report.Application.Repositories
{
    public interface IReportReadRepository
    {
        Task<PagedResult<Domain.Entities.Report>> GetAllReportByCompanyId(Guid companyId, int pageSize,
            int page = 1,
            bool trackChanges = false);
        Task<Domain.Entities.Report> GetReportById(Guid reportId, bool trackChanges = false);
    }
}
