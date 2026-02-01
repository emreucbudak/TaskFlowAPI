using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Infrastructure.Data.ProjectManagementDb;

namespace ProjectManagement.Infrastructure.Data.Repositories
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        Guid? DeletedBy { get; set; }
    }

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

    public interface ICurrentUserService
    {
        Guid UserId { get; }
        bool IsAuthenticated { get; }
    }

    public sealed class ProjectManagementWriteRepository(
        ProjectManagementDbContext context,
        ILogger<ProjectManagementWriteRepository> logger,
        ICurrentUserService currentUserService) : IProjectManagementWriteRepository
    {
        private const int MaxTaskTitleLength = 200;
        private const int MaxTaskDescriptionLength = 5000;
        private const int MinPriority = 1;
        private const int MaxPriority = 5;

        private static readonly string[] ValidStatuses =
            { "Todo", "InProgress", "Done", "Blocked", "OnHold" };

        public void AddTask(Domain.Entities.Task task)
        {
            ArgumentNullException.ThrowIfNull(task);

            ValidateTaskTitle(task.TaskName);
            ValidateTaskDescription(task.Description);
            ValidateProjectId(task.ProjectId);
            ValidateAssignedUserId(task.AssignedUserId);
            ValidateTaskStatus(task.TaskStatusId);
            ValidateTaskPriority(task.TaskPriority);

            if (task is ICreatableEntity creatable)
            {
                creatable.CreatedDate = DateTime.UtcNow;
                creatable.CreatedBy = currentUserService.UserId;
            }
            else if (task.CreatedDate == default)
            {
                task.CreatedDate = DateTime.UtcNow;
            }

            logger.LogInformation(
                "Task ekleniyor - Başlık: '{Title}', ProjeId: {ProjectId}, Atanan: {UserId}, Oluşturan: {CreatedBy}",
                task.TaskName,
                task.ProjectId,
                task.AssignedUserId,
                currentUserService.UserId);

            context.Tasks.Add(task);

            logger.LogDebug("Task context'e eklendi - Id: {TaskId}", task.Id);
        }

        public void UpdateTask(Domain.Entities.Task task)
        {
            ArgumentNullException.ThrowIfNull(task);
            ValidateId(task.Id);

            ValidateTaskTitle(task.TaskName);
            ValidateTaskDescription(task.Description);
            ValidateProjectId(task.ProjectId);
            ValidateTaskStatus(task.TaskStatusId);
            ValidateTaskPriority(task.TaskPriority);

            if (task is IUpdatableEntity updatable)
            {
                updatable.UpdatedDate = DateTime.UtcNow;
                updatable.UpdatedBy = currentUserService.UserId;
            }

            logger.LogInformation(
                "Task güncelleniyor - Id: {TaskId}, Başlık: '{Title}', Güncelleyen: {UserId}",
                task.Id,
                task.TaskName,
                currentUserService.UserId);

            var entry = context.Entry(task);

            if (entry.State == EntityState.Detached)
            {
                context.Tasks.Attach(task);
                entry.State = EntityState.Modified;
            }
            else
            {
                context.Tasks.Update(task);
            }

            logger.LogDebug("Task güncellendi - Id: {TaskId}", task.Id);
        }

        public void DeleteTask(Domain.Entities.Task task)
        {
            ArgumentNullException.ThrowIfNull(task);
            ValidateId(task.Id);

            if (task is ISoftDelete softDelete)
            {
                softDelete.IsDeleted = true;
                softDelete.DeletedAt = DateTime.UtcNow;
                softDelete.DeletedBy = currentUserService.UserId;

                logger.LogInformation(
                    "Task soft delete ediliyor - Id: {TaskId}, Başlık: '{Title}', Silen: {UserId}",
                    task.Id,
                    task.TaskName,
                    currentUserService.UserId);

                context.Tasks.Update(task);
            }
            else
            {
                logger.LogWarning(
                    "Task KALICI olarak siliniyor - Id: {TaskId}, Başlık: '{Title}', Silen: {UserId}",
                    task.Id,
                    task.TaskName,
                    currentUserService.UserId);

                context.Tasks.Remove(task);
            }

            logger.LogDebug("Task silindi - Id: {TaskId}", task.Id);
        }

        public void PermanentDeleteTask(Domain.Entities.Task task)
        {
            ArgumentNullException.ThrowIfNull(task);
            ValidateId(task.Id);

            logger.LogCritical(
                "KALICI SİLME - TaskId: {TaskId}, Başlık: '{Title}', Silen: {UserId}",
                task.Id,
                task.TaskName,
                currentUserService.UserId);

            context.Tasks.Remove(task);
        }

        private static void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Task ID'si boş olamaz", nameof(id));
            }
        }

        private static void ValidateTaskTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Task başlığı boş olamaz", nameof(title));
            }

            if (title.Length > MaxTaskTitleLength)
            {
                throw new ArgumentException(
                    $"Task başlığı {MaxTaskTitleLength} karakteri geçemez",
                    nameof(title));
            }
        }

        private static void ValidateTaskDescription(string? description)
        {
            if (!string.IsNullOrEmpty(description) && description.Length > MaxTaskDescriptionLength)
            {
                throw new ArgumentException(
                    $"Task açıklaması {MaxTaskDescriptionLength} karakteri geçemez",
                    nameof(description));
            }
        }

        private static void ValidateProjectId(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentException("Task bir projeye ait olmalıdır", nameof(projectId));
            }
        }

        private static void ValidateAssignedUserId(Guid? assignedUserId)
        {
            if (assignedUserId.HasValue && assignedUserId.Value == Guid.Empty)
            {
                throw new ArgumentException(
                    "Atanan kullanıcı ID'si boş GUID olamaz",
                    nameof(assignedUserId));
            }
        }

        private static void ValidateTaskStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Task durumu boş olamaz", nameof(status));
            }

            if (!ValidStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"Geçersiz durum '{status}'. Geçerli değerler: {string.Join(", ", ValidStatuses)}",
                    nameof(status));
            }
        }

        private static void ValidateTaskPriority(int priority)
        {
            if (priority < MinPriority || priority > MaxPriority)
            {
                throw new ArgumentException(
                    $"Geçersiz öncelik {priority}. Geçerli aralık: {MinPriority} (En Yüksek) - {MaxPriority} (En Düşük)",
                    nameof(priority));
            }
        }
    }
}