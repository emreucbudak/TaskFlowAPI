using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Infrastructure.Data.ProjectManagementDb;

namespace ProjectManagement.Infrastructure.Data.Repositories
{
    public class ProjectManagementReadRepository(
        ProjectManagementDbContext context,
        ILogger<ProjectManagementReadRepository> logger) : IProjectManagementReadRepository
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;

        /// <summary>
        /// Tüm task'ları sayfalı olarak getirir (GÜVENLI VERSİYON)
        /// NOT: GetAllTasks() yerine pagination zorunlu yapıldı
        /// </summary>
        public async Task<List<Domain.Entities.Task>> GetAllTasks(
            bool trackChanges,
            int page = 1,
            int pageSize = DefaultPageSize)
        {
            // Pagination validation
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Fetching all tasks - Page: {Page}, PageSize: {PageSize}, TrackChanges: {TrackChanges}",
                page, pageSize, trackChanges);

            IQueryable<Domain.Entities.Task> query = context.Tasks;

            // Soft delete filtresi (eğer Task entity'de IsDeleted varsa)
            // query = query.Where(t => !t.IsDeleted);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            // Varsayılan sıralama (oluşturma tarihi veya ID'ye göre)
            query = query.OrderByDescending(t => t.CreatedDate);

            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "Retrieved {Count} tasks - Page: {Page}",
                tasks.Count, page);

            return tasks;
        }


        [Obsolete("Use GetAllTasks(trackChanges, page, pageSize) instead. This method will be removed in future versions.")]
        public async Task<List<Domain.Entities.Task>> GetAllTasks(bool trackChanges)
        {
            logger.LogWarning(
                "DEPRECATED METHOD CALLED: GetAllTasks without pagination. Please update to use pagination.");

            // Güvenlik için ilk 100 kaydı getir
            return await GetAllTasks(trackChanges, 1, MaxPageSize);
        }


        public async Task<Domain.Entities.Task?> GetTask(Guid id, bool trackChanges)
        {
       
            ValidateId(id);

            logger.LogInformation(
                "Fetching task - Id: {TaskId}, TrackChanges: {TrackChanges}",
                id, trackChanges);

            IQueryable<Domain.Entities.Task> query = context.Tasks;


            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            var task = await query.FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                logger.LogWarning(
                    "Task not found - Id: {TaskId}",
                    id);
            }
            else
            {
                logger.LogDebug(
                    "Task found - Id: {TaskId}, Title: {Title}",
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
            // Input validation
            ValidateUserId(userId);
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Fetching tasks by userId - UserId: {UserId}, Page: {Page}, PageSize: {PageSize}",
                userId, page, pageSize);

            IQueryable<Domain.Entities.Task> query = context.Tasks
                .Where(t => t. == userId);

            // Soft delete filtresi
            // query = query.Where(t => !t.IsDeleted);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            query = query.OrderByDescending(t => t.CreatedDate);

            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "Retrieved {Count} tasks for user {UserId}",
                tasks.Count, userId);

            return tasks;
        }

        /// <summary>
        /// Proje ID'sine göre task'ları getirir
        /// </summary>
        public async Task<List<Domain.Entities.Task>> GetTasksByProjectId(
            Guid projectId,
            int page = 1,
            int pageSize = DefaultPageSize,
            bool trackChanges = false)
        {
            // Input validation
            ValidateProjectId(projectId);
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Fetching tasks by projectId - ProjectId: {ProjectId}, Page: {Page}, PageSize: {PageSize}",
                projectId, page, pageSize);

            IQueryable<Domain.Entities.Task> query = context.Tasks
                .Where(t => t.ProjectId == projectId);

            // Soft delete filtresi
            // query = query.Where(t => !t.IsDeleted);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            query = query.OrderBy(t => t.TaskPriority)
                         .ThenByDescending(t => t.CreatedDate);

            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "Retrieved {Count} tasks for project {ProjectId}",
                tasks.Count, projectId);

            return tasks;
        }

        /// <summary>
        /// Task durumuna göre filtreleme
        /// </summary>
        public async Task<List<Domain.Entities.Task>> GetTasksByStatus(
            string status,
            int page = 1,
            int pageSize = DefaultPageSize,
            bool trackChanges = false)
        {
            // Input validation
            ValidateStatus(status);
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Fetching tasks by status - Status: {Status}, Page: {Page}, PageSize: {PageSize}",
                status, page, pageSize);

            IQueryable<Domain.Entities.Task> query = context.Tasks
                .Where(t => t.TaskStatusId == status);

            // Soft delete filtresi
            // query = query.Where(t => !t.IsDeleted);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            query = query.OrderByDescending(t => t.CreatedDate);

            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "Retrieved {Count} tasks with status {Status}",
                tasks.Count, status);

            return tasks;
        }

        /// <summary>
        /// Toplam task sayısını getirir
        /// </summary>
        public async Task<int> GetTotalTaskCount()
        {
            logger.LogDebug("Getting total task count");

            var count = await context.Tasks
                .AsNoTracking()
                // .Where(t => !t.IsDeleted) // Soft delete varsa
                .CountAsync();

            logger.LogInformation("Total task count: {Count}", count);

            return count;
        }

        /// <summary>
        /// Task'ın var olup olmadığını kontrol eder
        /// </summary>
        public async Task<bool> TaskExists(Guid id)
        {
            ValidateId(id);

            logger.LogDebug("Checking if task exists - Id: {TaskId}", id);

            var exists = await context.Tasks
                .AsNoTracking()
                // .Where(t => !t.IsDeleted) // Soft delete varsa
                .AnyAsync(t => t.Id == id);

            logger.LogDebug(
                "Task exists check - Id: {TaskId}, Exists: {Exists}",
                id, exists);

            return exists;
        }

        #region Private Validation Methods

        private void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                logger.LogError("Empty task ID provided");
                throw new ArgumentException("Task ID cannot be empty", nameof(id));
            }
        }

        private void ValidateUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                logger.LogError("Empty user ID provided");
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }
        }

        private void ValidateProjectId(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                logger.LogError("Empty project ID provided");
                throw new ArgumentException("Project ID cannot be empty", nameof(projectId));
            }
        }

        private void ValidateStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                logger.LogError("Empty or null status provided");
                throw new ArgumentException("Status cannot be empty", nameof(status));
            }

            // Opsiyonel: İzin verilen status değerlerini kontrol et
            var validStatuses = new[] { "Todo", "InProgress", "Done", "Blocked" };
            if (!validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                logger.LogWarning(
                    "Invalid status value provided: {Status}. Valid values: {ValidStatuses}",
                    status, string.Join(", ", validStatuses));

                // Hata fırlatmak yerine warning verip devam edebilirsin
                // throw new ArgumentException($"Invalid status. Valid values: {string.Join(", ", validStatuses)}", nameof(status));
            }
        }

        private void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1)
            {
                logger.LogWarning("Invalid page number: {Page}, resetting to 1", page);
                page = 1;
            }

            if (pageSize < 1)
            {
                logger.LogWarning(
                    "Invalid page size: {PageSize}, resetting to default {DefaultPageSize}",
                    pageSize, DefaultPageSize);
                pageSize = DefaultPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning(
                    "Page size {PageSize} exceeds maximum {MaxPageSize}, limiting",
                    pageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }
        }

        #endregion
    }
}