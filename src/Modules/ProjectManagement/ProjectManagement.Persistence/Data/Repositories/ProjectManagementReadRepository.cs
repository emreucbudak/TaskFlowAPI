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

        public async Task<List<Task>> GetAllTasks(bool trackChanges, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
            {
                logger.LogWarning("Geçersiz sayfa numarası {PageNumber}. 1 kullanılıyor", pageNumber);
                pageNumber = 1;
            }

            if (pageSize < MinPageSize)
            {
                logger.LogWarning("Geçersiz sayfa boyutu {PageSize}. Minimum değer kullanılıyor: {MinPageSize}", pageSize, MinPageSize);
                pageSize = MinPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning("Sayfa boyutu {PageSize} maksimum değeri aşıyor. Kullanılan: {MaxPageSize}", pageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }

            try
            {
                logger.LogInformation("Task kayıtları getiriliyor. Sayfa: {PageNumber}, Boyut: {PageSize}, Takip: {TrackChanges}",
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

                var results = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                logger.LogInformation("{Count} adet Task kaydı başarıyla getirildi", results.Count);

                return results;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Task sorgusu iptal edildi");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Task kayıtları getirilirken hata oluştu. Sayfa: {PageNumber}, Boyut: {PageSize}", pageNumber, pageSize);
                throw;
            }
        }

        public async Task<Task> GetTask(Guid id, bool trackChanges)
        {
            if (id == Guid.Empty)
            {
                logger.LogWarning("Task için boş ID sağlandı");
                throw new ArgumentException("ID boş olamaz", nameof(id));
            }

            try
            {
                logger.LogInformation("Task kaydı getiriliyor. ID: {Id}, Takip: {TrackChanges}", id, trackChanges);

                var query = context.Tasks
                    .Include(t => t.subtask)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskPriority)
                    .AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var entity = await query.FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    logger.LogWarning("Task bulunamadı. ID: {Id}", id);
                    throw new KeyNotFoundException($"Task bulunamadı. ID: {id}");
                }

                logger.LogInformation("Task kaydı başarıyla getirildi. ID: {Id}", id);

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
                logger.LogError(ex, "Task kaydı getirilirken hata oluştu. ID: {Id}", id);
                throw;
            }
        }

        public async System.Threading.Tasks.Task<List<Domain.Entities.IndividualTasks>> GetAllIndividualTasks(bool trackChanges, int pageNumber, int pageSize,Guid userId)
        {
             if (pageNumber < 1)
            {
                logger.LogWarning("Geçersiz sayfa numarası {PageNumber}. 1 kullanılıyor", pageNumber);
                pageNumber = 1;
            }

            if (pageSize < MinPageSize)
            {
                logger.LogWarning("Geçersiz sayfa boyutu {PageSize}. Minimum değer kullanılıyor: {MinPageSize}", pageSize, MinPageSize);
                pageSize = MinPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning("Sayfa boyutu {PageSize} maksimum değeri aşıyor. Kullanılan: {MaxPageSize}", pageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }

            try
            {
                logger.LogInformation("IndividualTask kayıtları getiriliyor. Sayfa: {PageNumber}, Boyut: {PageSize}, Takip: {TrackChanges}",
                     pageNumber, pageSize, trackChanges);

                var query = context.IndividualTasks.AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var results = await query
                    .Where(it => it.AssignedUserId == userId)
                    .ToListAsync();

                logger.LogInformation("{Count} adet IndividualTask kaydı başarıyla getirildi", results.Count);

                return results;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("IndividualTask sorgusu iptal edildi");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "IndividualTask kayıtları getirilirken hata oluştu. Sayfa: {PageNumber}, Boyut: {PageSize}", pageNumber, pageSize);
                throw;
            }
        }

        public async System.Threading.Tasks.Task<Domain.Entities.IndividualTasks> GetIndividualTask(Guid id, bool trackChanges)
        {
             if (id == Guid.Empty)
            {
                logger.LogWarning("IndividualTask için boş ID sağlandı");
                throw new ArgumentException("ID boş olamaz", nameof(id));
            }

            try
            {
                logger.LogInformation("IndividualTask kaydı getiriliyor. ID: {Id}, Takip: {TrackChanges}", id, trackChanges);

                var query = context.IndividualTasks.AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var entity = await query.FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                {
                    logger.LogWarning("IndividualTask bulunamadı. ID: {Id}", id);
                    throw new KeyNotFoundException($"IndividualTask bulunamadı. ID: {id}");
                }

                logger.LogInformation("IndividualTask kaydı başarıyla getirildi. ID: {Id}", id);

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
                logger.LogError(ex, "IndividualTask kaydı getirilirken hata oluştu. ID: {Id}", id);
                throw;
            }
        }

    }
}
