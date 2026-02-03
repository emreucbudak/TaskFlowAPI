namespace Report.Application.Repositories
{
    public interface IReportWriteRepository
    {
        Task AddAsync(Report.Domain.Entities.Report report);
        Task UpdateAsync(Report.Domain.Entities.Report report);
        Task DeleteAsync(Report.Domain.Entities.Report report);
    }
}
