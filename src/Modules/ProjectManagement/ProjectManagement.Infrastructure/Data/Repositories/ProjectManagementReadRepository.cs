using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Infrastructure.Data.ProjectManagementDb;
using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Infrastructure.Data.Repositories
{
    public class ProjectManagementReadRepository<T>(
        ProjectManagementDbContext context,
        ILogger<ProjectManagementReadRepository<T>> logger)
        : IProjectManagementReadRepository<T>
        where T : BaseEntity
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;
        private const int MinPageSize = 1;

        public async Task<List<T>> GetAllTasks(bool trackChanges, int pageSize)
        {
            if (pageSize < MinPageSize)
            {
                logger.LogWarning(
                    "{EntityType} için geçersiz sayfa boyutu {PageSize}. Minimum değer kullanılıyor: {MinPageSize}",
                    typeof(T).Name, pageSize, MinPageSize);
                pageSize = MinPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning(
                    "{EntityType} için sayfa boyutu {PageSize} maksimum değeri aşıyor. Kullanılan: {MaxPageSize}",
                    typeof(T).Name, pageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }

            try
            {
                logger.LogInformation(
                    "{EntityType} kayıtları getiriliyor. Sayfa Boyutu: {PageSize}, Takip: {TrackChanges}",
                    typeof(T).Name, pageSize, trackChanges);

                var query = context.Set<T>().AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var results = await query
                    .Take(pageSize)
                    .ToListAsync();

                logger.LogInformation(
                    "{Count} adet {EntityType} kaydı başarıyla getirildi",
                    results.Count, typeof(T).Name);

                return results;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("{EntityType} sorgusu iptal edildi", typeof(T).Name);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "{EntityType} kayıtları getirilirken hata oluştu. Sayfa Boyutu: {PageSize}",
                    typeof(T).Name, pageSize);
                throw;
            }
        }

        public async Task<T> GetTask(Guid id, bool trackChanges)
        {
            if (id == Guid.Empty)
            {
                logger.LogWarning(
                    "{EntityType} için boş ID sağlandı",
                    typeof(T).Name);
                throw new ArgumentException("ID boş olamaz", nameof(id));
            }

            try
            {
                logger.LogInformation(
                    "{EntityType} kaydı getiriliyor. ID: {Id}, Takip: {TrackChanges}",
                    typeof(T).Name, id, trackChanges);

                var query = context.Set<T>().AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var entity = await query
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    logger.LogWarning(
                        "{EntityType} bulunamadı. ID: {Id}",
                        typeof(T).Name, id);
                    throw new KeyNotFoundException(
                        $"{typeof(T).Name} bulunamadı. ID: {id}");
                }

                logger.LogInformation(
                    "{EntityType} kaydı başarıyla getirildi. ID: {Id}",
                    typeof(T).Name, id);

                return entity;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning(
                    "{EntityType} sorgusu iptal edildi. ID: {Id}",
                    typeof(T).Name, id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "{EntityType} kaydı getirilirken hata oluştu. ID: {Id}",
                    typeof(T).Name, id);
                throw;
            }
        }
        public async Task<List<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            bool trackChanges = false)
        {
            if (pageNumber < 1)
            {
                logger.LogWarning(
                    "{EntityType} için geçersiz sayfa numarası {PageNumber}. 1 kullanılıyor",
                    typeof(T).Name, pageNumber);
                pageNumber = 1;
            }

            if (pageSize < MinPageSize || pageSize > MaxPageSize)
            {
                logger.LogWarning(
                    "{EntityType} için geçersiz sayfa boyutu {PageSize}. Varsayılan kullanılıyor: {DefaultPageSize}",
                    typeof(T).Name, pageSize, DefaultPageSize);
                pageSize = DefaultPageSize;
            }

            try
            {
                var query = context.Set<T>().AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                logger.LogInformation(
                    "{EntityType} sayfa {PageNumber} getirildi. Sayfa Boyutu: {PageSize}, Toplam: {TotalCount}",
                    typeof(T).Name, pageNumber, pageSize, totalCount);

                return items;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "{EntityType} sayfalama sırasında hata oluştu. Sayfa: {PageNumber}, Boyut: {PageSize}",
                    typeof(T).Name, pageNumber, pageSize);
                throw;
            }
        }
    }
}