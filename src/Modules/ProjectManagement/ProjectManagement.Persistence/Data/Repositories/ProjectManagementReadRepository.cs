using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Persistence.Data.ProjectManagementDb;
using Task = ProjectManagement.Domain.Entities.Task;

namespace ProjectManagement.Persistence.Data.Repositories
{
    public class ProjectManagementReadRepository(
        ProjectManagementDbContext context,
        ILogger<ProjectManagementReadRepository> logger)
        : IProjectManagementReadRepository
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;
        private const int MinPageSize = 1;

        public async Task<(List<Task> Items, int TotalCount)> GetAllTasks(bool trackChanges, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            if (pageNumber < 1)
            {
                logger.LogWarning("Gecersiz sayfa numarasi {PageNumber}. 1 kullaniliyor", pageNumber);
                pageNumber = 1;
            }

            if (pageSize < MinPageSize)
            {
                logger.LogWarning("Gecersiz sayfa boyutu {PageSize}. Minimum deger kullaniliyor: {MinPageSize}", pageSize, MinPageSize);
                pageSize = MinPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning("Sayfa boyutu {PageSize} maksimum degeri asiyor. Kullanilan: {MaxPageSize}", pageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }

            try
            {
                logger.LogInformation("Task kayitlari getiriliyor. Sayfa: {PageNumber}, Boyut: {PageSize}, Takip: {TrackChanges}",
                     pageNumber, pageSize, trackChanges);

                var query = context.Tasks
                    .Include(t => t.subtask)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var totalCount = await query.CountAsync(cancellationToken);

                var results = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                logger.LogInformation("{Count} adet Task kaydi basariyla getirildi", results.Count);

                return (results, totalCount);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Task sorgusu iptal edildi");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Task kayitlari getirilirken hata olustu. Sayfa: {PageNumber}, Boyut: {PageSize}", pageNumber, pageSize);
                throw;
            }
        }

        public async Task<Task> GetTask(Guid id, bool trackChanges, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                logger.LogWarning("Task icin bos ID saglandi");
                throw new ArgumentException("ID bos olamaz", nameof(id));
            }

            try
            {
                logger.LogInformation("Task kaydi getiriliyor. ID: {Id}, Takip: {TrackChanges}", id, trackChanges);

                var query = context.Tasks
                    .Include(t => t.subtask)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var entity = await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (entity == null)
                {
                    logger.LogWarning("Task bulunamadi. ID: {Id}", id);
                    throw new KeyNotFoundException($"Task bulunamadi. ID: {id}");
                }

                logger.LogInformation("Task kaydi basariyla getirildi. ID: {Id}", id);

                return entity;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Task sorgusu iptal edildi. ID: {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Task kaydi getirilirken hata olustu. ID: {Id}", id);
                throw;
            }
        }

        public async Task<(List<Domain.Entities.IndividualTasks> Items, int TotalCount)> GetAllIndividualTasks(bool trackChanges, int pageNumber, int pageSize, Guid userId, CancellationToken cancellationToken)
        {
            if (pageNumber < 1)
            {
                logger.LogWarning("Gecersiz sayfa numarasi {PageNumber}. 1 kullaniliyor", pageNumber);
                pageNumber = 1;
            }

            if (pageSize < MinPageSize)
            {
                logger.LogWarning("Gecersiz sayfa boyutu {PageSize}. Minimum deger kullaniliyor: {MinPageSize}", pageSize, MinPageSize);
                pageSize = MinPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning("Sayfa boyutu {PageSize} maksimum degeri asiyor. Kullanilan: {MaxPageSize}", pageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }

            try
            {
                logger.LogInformation("IndividualTask kayitlari getiriliyor. Sayfa: {PageNumber}, Boyut: {PageSize}, Takip: {TrackChanges}",
                     pageNumber, pageSize, trackChanges);

                var query = context.IndividualTasks
                    .Include(it => it.TaskPriority)
                    .Where(it => it.AssignedUserId == userId)
                    .AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var totalCount = await query.CountAsync(cancellationToken);

                var results = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                logger.LogInformation("{Count} adet IndividualTask kaydi basariyla getirildi", results.Count);

                return (results, totalCount);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("IndividualTask sorgusu iptal edildi");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "IndividualTask kayitlari getirilirken hata olustu. Sayfa: {PageNumber}, Boyut: {PageSize}", pageNumber, pageSize);
                throw;
            }
        }

        public async Task<Domain.Entities.IndividualTasks> GetIndividualTask(Guid id, bool trackChanges, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                logger.LogWarning("IndividualTask icin bos ID saglandi");
                throw new ArgumentException("ID bos olamaz", nameof(id));
            }

            try
            {
                logger.LogInformation("IndividualTask kaydi getiriliyor. ID: {Id}, Takip: {TrackChanges}", id, trackChanges);

                var query = context.IndividualTasks
                    .Include(it => it.TaskPriority)
                    .AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var entity = await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

                if (entity == null)
                {
                    logger.LogWarning("IndividualTask bulunamadi. ID: {Id}", id);
                    throw new KeyNotFoundException($"IndividualTask bulunamadi. ID: {id}");
                }

                logger.LogInformation("IndividualTask kaydi basariyla getirildi. ID: {Id}", id);

                return entity;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("IndividualTask sorgusu iptal edildi. ID: {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "IndividualTask kaydi getirilirken hata olustu. ID: {Id}", id);
                throw;
            }
        }
    }
}
