namespace Identity.Application.Repositories
{
    public interface IWriteRepository<T> where T : class
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
