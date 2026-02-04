namespace Report.Application.Repositories
{
    public interface IReportWriteRepository
    {
        Task AddAsync(Domain.Entities.Report entity);
        void UpdateAsync(Domain.Entities.Report entity);
        void DeleteAsync(Domain.Entities.Report entity);
    }
}
