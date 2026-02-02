namespace Identity.Application.Repositories
{
    public interface IWriteRepository <T> where T : class 
    {
        Task AddAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);
    }
}
