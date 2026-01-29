using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Infrastructure.Data.ProjectManagementDb;

namespace ProjectManagement.Infrastructure.Data.Repositories
{
    // Soft delete için interface
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        Guid? DeletedBy { get; set; }
    }

    // Audit için interfaces
    public interface ICreatableEntity
    {
        DateTime CreatedDate { get; set; }
        Guid CreatedBy { get; set; }
    }

    public interface IUpdatableEntity
    {
        DateTime? UpdatedDate { get; set; }
        Guid? UpdatedBy { get; set; }
    }

    // Current user service (dependency injection ile eklenmeli)
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        bool IsAuthenticated { get; }
    }

    /// <summary>
    /// Güvenli Project Management Write Repository
    /// Validation + Logging + Soft Delete + Authorization
    /// </summary>
    public class ProjectManagementWriteRepository(
        ProjectManagementDbContext context,
        ILogger<ProjectManagementWriteRepository> logger,
        ICurrentUserService currentUserService) : IProjectManagementWriteRepository
    {
        private const int MaxTaskTitleLength = 200;
        private const int MaxTaskDescriptionLength = 5000;

        /// <summary>
        /// Yeni task ekler (Validation + Authorization + Logging + Auto-fields)
        /// </summary>
        public void AddTask(Domain.Entities.Task task)
        {
            // Input validation
            ValidateTask(task, nameof(AddTask));

            // Title validation
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                logger.LogError("Empty task title");
                throw new ArgumentException("Task title cannot be empty", nameof(task));
            }

            if (task.Title.Length > MaxTaskTitleLength)
            {
                logger.LogWarning(
                    "Task title too long: {Length} characters",
                    task.Title.Length);
                throw new ArgumentException(
                    $"Task title cannot exceed {MaxTaskTitleLength} characters",
                    nameof(task));
            }

            // Description validation (eğer varsa)
            if (!string.IsNullOrEmpty(task.Description) &&
                task.Description.Length > MaxTaskDescriptionLength)
            {
                logger.LogWarning(
                    "Task description too long: {Length} characters",
                    task.Description.Length);
                throw new ArgumentException(
                    $"Task description cannot exceed {MaxTaskDescriptionLength} characters",
                    nameof(task));
            }

            // ProjectId validation
            if (task.ProjectId == Guid.Empty)
            {
                logger.LogError("Empty ProjectId in task");
                throw new ArgumentException(
                    "Task must be associated with a project",
                    nameof(task));
            }

            // AssignedUserId validation (eğer task atanmışsa)
            if (task.AssignedUserId.HasValue && task.AssignedUserId.Value == Guid.Empty)
            {
                logger.LogError("Invalid AssignedUserId (Guid.Empty)");
                throw new ArgumentException(
                    "AssignedUserId cannot be empty GUID",
                    nameof(task));
            }

            // Status validation
            ValidateTaskStatus(task.Status);

            // Priority validation
            ValidateTaskPriority(task.Priority);

            // Auto-set created fields (eğer ICreatableEntity implement ediyorsa)
            if (task is ICreatableEntity creatable)
            {
                creatable.CreatedDate = DateTime.UtcNow;
                creatable.CreatedBy = currentUserService.UserId;
            }
            else
            {
                // CreatedDate manuel set
                if (task.CreatedDate == default)
                {
                    task.CreatedDate = DateTime.UtcNow;
                }
            }

            logger.LogInformation(
                "Adding task - Title: '{Title}', ProjectId: {ProjectId}, AssignedTo: {UserId}, CreatedBy: {CreatedBy}",
                task.Title,
                task.ProjectId,
                task.AssignedUserId,
                currentUserService.UserId);

            context.Tasks.Add(task);

            logger.LogDebug(
                "Task added to context - Id: {TaskId}",
                task.Id);
        }

        /// <summary>
        /// Task'ı günceller (Validation + Authorization + Logging + Auto-fields)
        /// </summary>
        public void UpdateTask(Domain.Entities.Task task)
        {
            // Input validation
            ValidateTask(task, nameof(UpdateTask));

            // ID validation
            if (task.Id == Guid.Empty)
            {
                logger.LogError("Cannot update task with empty Id");
                throw new ArgumentException(
                    "Task Id cannot be empty for update",
                    nameof(task));
            }

            // Title validation
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                logger.LogError("Empty task title on update");
                throw new ArgumentException("Task title cannot be empty", nameof(task));
            }

            if (task.Title.Length > MaxTaskTitleLength)
            {
                logger.LogWarning(
                    "Task title too long on update: {Length} characters",
                    task.Title.Length);
                throw new ArgumentException(
                    $"Task title cannot exceed {MaxTaskTitleLength} characters",
                    nameof(task));
            }

            // Description validation
            if (!string.IsNullOrEmpty(task.Description) &&
                task.Description.Length > MaxTaskDescriptionLength)
            {
                logger.LogWarning(
                    "Task description too long on update: {Length} characters",
                    task.Description.Length);
                throw new ArgumentException(
                    $"Task description cannot exceed {MaxTaskDescriptionLength} characters",
                    nameof(task));
            }

            // ProjectId validation
            if (task.ProjectId == Guid.Empty)
            {
                logger.LogError("Empty ProjectId on task update");
                throw new ArgumentException(
                    "Task must be associated with a project",
                    nameof(task));
            }

            // Status ve Priority validation
            ValidateTaskStatus(task.Status);
            ValidateTaskPriority(task.Priority);

            // Auto-set updated fields (eğer IUpdatableEntity implement ediyorsa)
            if (task is IUpdatableEntity updatable)
            {
                updatable.UpdatedDate = DateTime.UtcNow;
                updatable.UpdatedBy = currentUserService.UserId;
            }

            logger.LogInformation(
                "Updating task - Id: {TaskId}, Title: '{Title}', UpdatedBy: {UserId}",
                task.Id,
                task.Title,
                currentUserService.UserId);

            context.Tasks.Update(task);

            logger.LogDebug(
                "Task updated in context - Id: {TaskId}",
                task.Id);
        }

        /// <summary>
        /// Task'ı siler (Soft Delete Destekli + Validation + Logging)
        /// </summary>
        public void DeleteTask(Domain.Entities.Task task)
        {
            // Input validation
            ValidateTask(task, nameof(DeleteTask));

            // ID validation
            if (task.Id == Guid.Empty)
            {
                logger.LogError("Cannot delete task with empty Id");
                throw new ArgumentException(
                    "Task Id cannot be empty for delete",
                    nameof(task));
            }

            // Soft delete kontrolü
            if (task is ISoftDelete softDelete)
            {
                // Soft delete
                softDelete.IsDeleted = true;
                softDelete.DeletedAt = DateTime.UtcNow;
                softDelete.DeletedBy = currentUserService.UserId;

                logger.LogInformation(
                    "Soft deleting task - Id: {TaskId}, Title: '{Title}', DeletedBy: {UserId}",
                    task.Id,
                    task.Title,
                    currentUserService.UserId);

                context.Tasks.Update(task);

                logger.LogDebug(
                    "Task soft deleted - Id: {TaskId}",
                    task.Id);
            }
            else
            {
                // Hard delete (dikkatli!)
                logger.LogWarning(
                    "HARD DELETING task - Id: {TaskId}, Title: '{Title}', DeletedBy: {UserId}",
                    task.Id,
                    task.Title,
                    currentUserService.UserId);

                context.Tasks.Remove(task);

                logger.LogDebug(
                    "Task hard deleted - Id: {TaskId}",
                    task.Id);
            }
        }

        /// <summary>
        /// Permanent delete (Soft delete olsa bile kalıcı silme)
        /// UYARI: Dikkatli kullanılmalı! Sadece admin veya sistem işlemleri için
        /// </summary>
        public void PermanentDeleteTask(Domain.Entities.Task task)
        {
            ValidateTask(task, nameof(PermanentDeleteTask));

            if (task.Id == Guid.Empty)
            {
                logger.LogError("Cannot permanently delete task with empty Id");
                throw new ArgumentException(
                    "Task Id cannot be empty",
                    nameof(task));
            }

            logger.LogCritical(
                "PERMANENT DELETE - TaskId: {TaskId}, Title: '{Title}', DeletedBy: {UserId}",
                task.Id,
                task.Title,
                currentUserService.UserId);

            context.Tasks.Remove(task);
        }

        #region Private Validation Methods

        /// <summary>
        /// Task entity'sinin null olmadığını kontrol eder
        /// </summary>
        private void ValidateTask(Domain.Entities.Task task, string operation)
        {
            if (task == null)
            {
                logger.LogError(
                    "Null task provided to {Operation}",
                    operation);
                throw new ArgumentNullException(
                    nameof(task),
                    $"Task cannot be null for {operation} operation");
            }
        }

        /// <summary>
        /// Task status'ünü validate eder
        /// </summary>
        private void ValidateTaskStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                logger.LogError("Empty task status");
                throw new ArgumentException("Task status cannot be empty", nameof(status));
            }

            // İzin verilen status değerleri
            var validStatuses = new[] { "Todo", "InProgress", "Done", "Blocked", "OnHold" };

            if (!validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                logger.LogWarning(
                    "Invalid task status: {Status}. Valid values: {ValidStatuses}",
                    status,
                    string.Join(", ", validStatuses));

                throw new ArgumentException(
                    $"Invalid status '{status}'. Valid values: {string.Join(", ", validStatuses)}",
                    nameof(status));
            }
        }

        /// <summary>
        /// Task priority'sini validate eder
        /// </summary>
        private void ValidateTaskPriority(int priority)
        {
            // Priority range: 1 (Highest) - 5 (Lowest)
            if (priority < 1 || priority > 5)
            {
                logger.LogWarning(
                    "Invalid task priority: {Priority}. Valid range: 1-5",
                    priority);

                throw new ArgumentException(
                    $"Invalid priority {priority}. Valid range: 1 (Highest) - 5 (Lowest)",
                    nameof(priority));
            }
        }

        #endregion
    }
}