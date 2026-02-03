using Report.Application.Repositories;
using Report.Persistence.Data;

namespace Report.Persistence.Repositories
{
    public class ReportWriteRepository(ReportDbContext db) : IReportWriteRepository
    {
        public Task AddAsync(Domain.Entities.Report report)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Domain.Entities.Report report)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Domain.Entities.Report report)
        {
            throw new NotImplementedException();
        }
    }
}
