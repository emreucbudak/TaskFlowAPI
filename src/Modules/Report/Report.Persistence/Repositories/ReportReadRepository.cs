using Report.Application.Repositories;
using Report.Persistence.Data;
using TaskFlow.BuildingBlocks.Common;

namespace Report.Persistence.Repositories
{
    public class ReportReadRepository(ReportDbContext db) : IReportReadRepository
    {
        public Task<PagedResult<Domain.Entities.Report>> GetAllReportByCompanyId(Guid companyId, int pageSize, int page = 1, bool trackChanges = false)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Entities.Report> GetReportById(Guid reportId, bool trackChanges = false)
        {
            throw new NotImplementedException();
        }
    }
}
