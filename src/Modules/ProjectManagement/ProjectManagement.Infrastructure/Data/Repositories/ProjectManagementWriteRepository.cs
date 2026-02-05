using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Infrastructure.Data.ProjectManagementDb;
using Task = ProjectManagement.Domain.Entities.Task;

namespace ProjectManagement.Infrastructure.Data.Repositories
{
    public class ProjectManagementWriteRepository(
        ProjectManagementDbContext context,
        ILogger<ProjectManagementWriteRepository> logger
        ) : IProjectManagementWriteRepository
    {
        private const int MaxTaskTitleLength = 200;
        private const int MaxTaskDescriptionLength = 5000;
        private const int MinPriority = 1;
        private const int MaxPriority = 5;

        public async System.Threading.Tasks.Task AddTask(Task entity)
        {
            if (entity == null)
            {
                logger.LogError("Eklenmeye çalışılan task null");
                throw new ArgumentNullException(nameof(entity), "Entity null olamaz");
            }

            try
            {
                ValidateEntity(entity);

                await context.Tasks.AddAsync(entity);
                await context.SaveChangesAsync();

                logger.LogInformation("Task başarıyla eklendi. Id: {EntityId}", entity.Id);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Veritabanına kayıt eklenirken hata oluştu. Entity Id: {EntityId}", entity.Id);
                throw new InvalidOperationException("Veritabanına kayıt eklenirken bir hata oluştu", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Task eklenirken beklenmeyen hata. Entity Id: {EntityId}", entity.Id);
                throw;
            }
        }

        public async System.Threading.Tasks.Task DeleteTask(Task task)
        {
            if (task == null)
            {
                logger.LogError("Silinmeye çalışılan task null");
                throw new ArgumentNullException(nameof(task), "Task null olamaz");
            }

            try
            {
                var existingEntity = await context.Tasks
                    .FirstOrDefaultAsync(e => e.Id == task.Id);

                if (existingEntity == null)
                {
                    logger.LogWarning("Silinmek istenen task bulunamadı. Id: {TaskId}", task.Id);
                    throw new InvalidOperationException($"Id'si {task.Id} olan kayıt bulunamadı");
                }

                context.Tasks.Remove(existingEntity);
                await context.SaveChangesAsync();

                logger.LogInformation("Task başarıyla silindi. Id: {TaskId}", task.Id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogError(ex, "Task silinirken eşzamanlılık hatası. Id: {TaskId}", task.Id);
                throw new InvalidOperationException("Kayıt başka bir işlem tarafından değiştirilmiş", ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Task silinirken veritabanı hatası. Id: {TaskId}", task.Id);
                throw new InvalidOperationException("Kayıt silinirken veritabanı hatası oluştu", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Task silinirken beklenmeyen hata. Id: {TaskId}", task.Id);
                throw;
            }
        }

        public async System.Threading.Tasks.Task UpdateTask(Task task)
        {
            if (task == null)
            {
                logger.LogError("Güncellenmeye çalışılan task null");
                throw new ArgumentNullException(nameof(task), "Task null olamaz");
            }

            try
            {
                ValidateEntity(task);

                var existingEntity = await context.Tasks
                    .FirstOrDefaultAsync(e => e.Id == task.Id);

                if (existingEntity == null)
                {
                    logger.LogWarning("Güncellenmek istenen task bulunamadı. Id: {TaskId}", task.Id);
                    throw new InvalidOperationException($"Id'si {task.Id} olan kayıt bulunamadı");
                }

                context.Entry(existingEntity).CurrentValues.SetValues(task);
                await context.SaveChangesAsync();

                logger.LogInformation("Task başarıyla güncellendi. Id: {TaskId}", task.Id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogError(ex, "Task güncellenirken eşzamanlılık hatası. Id: {TaskId}", task.Id);
                throw new InvalidOperationException("Kayıt başka bir işlem tarafından değiştirilmiş", ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Task güncellenirken veritabanı hatası. Id: {TaskId}", task.Id);
                throw new InvalidOperationException("Kayıt güncellenirken veritabanı hatası oluştu", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Task güncellenirken beklenmeyen hata. Id: {TaskId}", task.Id);
                throw;
            }
        }

        private void ValidateEntity(Task entity)
        {
            if (entity.Id == Guid.Empty)
            {
                logger.LogError("Entity Id boş olamaz");
                throw new ArgumentException("Entity Id boş olamaz", nameof(entity));
            }

            if (string.IsNullOrWhiteSpace(entity.TaskName))
            {
                logger.LogError("Task başlığı boş olamaz. Entity Id: {EntityId}", entity.Id);
                throw new ArgumentException("Task başlığı boş olamaz", nameof(entity));
            }

            if (entity.TaskName.Length > MaxTaskTitleLength)
            {
                logger.LogError("Task başlığı maksimum {MaxLength} karakter olabilir. Mevcut: {CurrentLength}. Entity Id: {EntityId}",
                    MaxTaskTitleLength, entity.TaskName.Length, entity.Id);
                throw new ArgumentException($"Task başlığı maksimum {MaxTaskTitleLength} karakter olabilir", nameof(entity));
            }

            if (!string.IsNullOrWhiteSpace(entity.Description) && entity.Description.Length > MaxTaskDescriptionLength)
            {
                logger.LogError("Task açıklaması maksimum {MaxLength} karakter olabilir. Mevcut: {CurrentLength}. Entity Id: {EntityId}",
                    MaxTaskDescriptionLength, entity.Description.Length, entity.Id);
                throw new ArgumentException($"Task açıklaması maksimum {MaxTaskDescriptionLength} karakter olabilir", nameof(entity));
            }
        }
    }
}