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
        private const int MinPageSize = 1;

        public async Task<(List<Task> Items, int TotalCount)> GetAllTasks(bool trackChanges, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            pageNumber = NormalizePageNumber(pageNumber);
            pageSize = NormalizePageSize(pageSize);

            try
            {
                logger.LogInformation("Task kayitlari getiriliyor. Sayfa: {PageNumber}, Boyut: {PageSize}, Takip: {TrackChanges}",
                    pageNumber, pageSize, trackChanges);

                var query = BuildTaskQuery(trackChanges);
                var totalCount = await query.CountAsync(cancellationToken);

                var results = await query
                    .OrderBy(task => task.DeadlineTime)
                    .ThenBy(task => task.Id)
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

        public async Task<(List<Task> Items, int TotalCount)> GetTasksByAssignedUserIds(
            bool trackChanges,
            int pageNumber,
            int pageSize,
            IReadOnlyCollection<Guid> assignedUserIds,
            CancellationToken cancellationToken)
        {
            pageNumber = NormalizePageNumber(pageNumber);
            pageSize = NormalizePageSize(pageSize);

            var normalizedAssignedUserIds = assignedUserIds
                .Where(userId => userId != Guid.Empty)
                .Distinct()
                .ToArray();

            if (normalizedAssignedUserIds.Length == 0)
            {
                logger.LogInformation("Assigned user listesi bos geldigi icin grup gorevi sonucu bos donuldu.");
                return ([], 0);
            }

            try
            {
                logger.LogInformation(
                    "Assigned user ID filtreli task kayitlari getiriliyor. Sayfa: {PageNumber}, Boyut: {PageSize}, KullaniciSayisi: {AssignedUserCount}, Takip: {TrackChanges}",
                    pageNumber,
                    pageSize,
                    normalizedAssignedUserIds.Length,
                    trackChanges);

                var query = BuildTaskQuery(trackChanges)
                    .Where(task => task.subtask.Any(subTask => normalizedAssignedUserIds.Contains(subTask.AssignedUserId)));

                var totalCount = await query.CountAsync(cancellationToken);

                var results = await query
                    .OrderBy(task => task.DeadlineTime)
                    .ThenBy(task => task.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                logger.LogInformation("{Count} adet filtrelenmis grup gorevi kaydi basariyla getirildi", results.Count);

                return (results, totalCount);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Assigned user filtreli task sorgusu iptal edildi");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Assigned user filtreli task kayitlari getirilirken hata olustu. Sayfa: {PageNumber}, Boyut: {PageSize}", pageNumber, pageSize);
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

                var entity = await BuildTaskQuery(trackChanges)
                    .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);

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
            pageNumber = NormalizePageNumber(pageNumber);
            pageSize = NormalizePageSize(pageSize);

            try
            {
                logger.LogInformation("IndividualTask kayitlari getiriliyor. Sayfa: {PageNumber}, Boyut: {PageSize}, Takip: {TrackChanges}",
                    pageNumber, pageSize, trackChanges);

                var query = context.IndividualTasks
                    .Include(task => task.TaskPriority)
                    .Where(task => task.AssignedUserId == userId)
                    .AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var totalCount = await query.CountAsync(cancellationToken);

                var results = await query
                    .OrderBy(task => task.Deadline)
                    .ThenBy(task => task.Id)
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
                    .Include(task => task.TaskPriority)
                    .AsQueryable();

                if (!trackChanges)
                {
                    query = query.AsNoTracking();
                }

                var entity = await query.FirstOrDefaultAsync(task => task.Id == id, cancellationToken);

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

        private IQueryable<Task> BuildTaskQuery(bool trackChanges)
        {
            var query = context.Tasks
                .Include(task => task.subtask)
                .ThenInclude(subTask => subTask.TaskStatus)
                .Include(task => task.TaskStatus)
                .Include(task => task.TaskPriority)
                .AsQueryable();

            return trackChanges ? query : query.AsNoTracking();
        }

        private int NormalizePageNumber(int pageNumber)
        {
            if (pageNumber >= 1)
            {
                return pageNumber;
            }

            logger.LogWarning("Gecersiz sayfa numarasi {PageNumber}. 1 kullaniliyor", pageNumber);
            return 1;
        }

        private int NormalizePageSize(int pageSize)
        {
            if (pageSize < MinPageSize)
            {
                logger.LogWarning("Gecersiz sayfa boyutu {PageSize}. Minimum deger kullaniliyor: {MinPageSize}", pageSize, MinPageSize);
                return MinPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning("Sayfa boyutu {PageSize} maksimum degeri asiyor. Kullanilan: {MaxPageSize}", pageSize, MaxPageSize);
                return MaxPageSize;
            }

            return pageSize;
        }
    }
}