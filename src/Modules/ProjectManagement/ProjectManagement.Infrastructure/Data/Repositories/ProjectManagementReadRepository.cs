using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Infrastructure.Data.ProjectManagementDb;

namespace ProjectManagement.Infrastructure.Data.Repositories
{
    public sealed class ProjectManagementReadRepository(
        ProjectManagementDbContext context,
        ILogger<ProjectManagementReadRepository> logger) : IProjectManagementReadRepository
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;

        public async Task<List<Domain.Entities.Task>> GetAllTasks(
            bool trackChanges,
            int page = 1,
            int pageSize = DefaultPageSize)
        {
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Tüm task'lar getiriliyor - Sayfa: {Page}, Sayfa Boyutu: {PageSize}, Değişiklik İzleme: {TrackChanges}",
                page, pageSize, trackChanges);

            IQueryable<Domain.Entities.Task> query = context.Tasks;

            if (!trackChanges)
                query = query.AsNoTracking();

            query = query.OrderByDescending(t => t.CreatedDate);

            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "{Count} adet task getirildi - Sayfa: {Page}",
                tasks.Count, page);

            return tasks;
        }

        public async Task<Domain.Entities.Task?> GetTask(Guid id, bool trackChanges)
        {
            ValidateId(id);

            logger.LogInformation(
                "Task getiriliyor - Id: {TaskId}, Değişiklik İzleme: {TrackChanges}",
                id, trackChanges);

            IQueryable<Domain.Entities.Task> query = context.Tasks;

            if (!trackChanges)
                query = query.AsNoTracking();

            var task = await query.FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                logger.LogWarning("Task bulunamadı - Id: {TaskId}", id);
            }
            else
            {
                logger.LogDebug(
                    "Task bulundu - Id: {TaskId}, İsim: {Name}",
                    id, task.TaskName ?? "N/A");
            }

            return task;
        }

        public async Task<List<Domain.Entities.Task>> GetTasksByUserId(
            Guid userId,
            int page = 1,
            int pageSize = DefaultPageSize,
            bool trackChanges = false)
        {
            ValidateUserId(userId);
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Kullanıcıya ait task'lar getiriliyor - UserId: {UserId}, Sayfa: {Page}, Sayfa Boyutu: {PageSize}",
                userId, page, pageSize);

            IQueryable<Domain.Entities.Task> query = context.Tasks
                .Where(t => t.AssignedUserId == userId);

            if (!trackChanges)
                query = query.AsNoTracking();

            query = query.OrderByDescending(t => t.CreatedDate);

            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "Kullanıcı {UserId} için {Count} adet task getirildi",
                userId, tasks.Count);

            return tasks;
        }

        public async Task<List<Domain.Entities.Task>> GetTasksByProjectId(
            Guid projectId,
            int page = 1,
            int pageSize = DefaultPageSize,
            bool trackChanges = false)
        {
            ValidateProjectId(projectId);
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Projeye ait task'lar getiriliyor - ProjectId: {ProjectId}, Sayfa: {Page}, Sayfa Boyutu: {PageSize}",
                projectId, page, pageSize);

            IQueryable<Domain.Entities.Task> query = context.Tasks
                .Where(t => t.ProjectId == projectId);

            if (!trackChanges)
                query = query.AsNoTracking();

            query = query
                .OrderBy(t => t.TaskPriority)
                .ThenByDescending(t => t.CreatedDate);

            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "Proje {ProjectId} için {Count} adet task getirildi",
                projectId, tasks.Count);

            return tasks;
        }

        public async Task<List<Domain.Entities.Task>> GetTasksByStatus(
            string status,
            int page = 1,
            int pageSize = DefaultPageSize,
            bool trackChanges = false)
        {
            ValidateStatus(status);
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Duruma göre task'lar getiriliyor - Durum: {Status}, Sayfa: {Page}, Sayfa Boyutu: {PageSize}",
                status, page, pageSize);

            IQueryable<Domain.Entities.Task> query = context.Tasks
                .Where(t => t.TaskStatusId == status);

            if (!trackChanges)
                query = query.AsNoTracking();

            query = query.OrderByDescending(t => t.CreatedDate);

            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "{Status} durumunda {Count} adet task getirildi",
                status, tasks.Count);

            return tasks;
        }

        public async Task<int> GetTotalTaskCount()
        {
            logger.LogDebug("Toplam task sayısı alınıyor");

            var count = await context.Tasks
                .AsNoTracking()
                .CountAsync();

            logger.LogInformation("Toplam task sayısı: {Count}", count);

            return count;
        }

        public async Task<bool> TaskExists(Guid id)
        {
            ValidateId(id);

            logger.LogDebug("Task varlığı kontrol ediliyor - Id: {TaskId}", id);

            var exists = await context.Tasks
                .AsNoTracking()
                .AnyAsync(t => t.Id == id);

            logger.LogDebug(
                "Task varlık kontrolü - Id: {TaskId}, Var mı: {Exists}",
                id, exists);

            return exists;
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Task ID'si boş olamaz", nameof(id));
            }
        }

        private static void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Kullanıcı ID'si boş olamaz", nameof(userId));
            }
        }

        private static void ValidateProjectId(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentException("Proje ID'si boş olamaz", nameof(projectId));
            }
        }

        private static void ValidateStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Durum boş olamaz", nameof(status));
            }
        }

        private static void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = DefaultPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                pageSize = MaxPageSize;
            }
        }
    }
}