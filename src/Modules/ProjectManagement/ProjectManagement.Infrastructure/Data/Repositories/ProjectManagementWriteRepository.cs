using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Repositories;
using ProjectManagement.Infrastructure.Data.ProjectManagementDb;
using TaskFlow.BuildingBlocks.Common;

namespace ProjectManagement.Infrastructure.Data.Repositories
{
    public class ProjectManagementWriteRepository<T>(
        ProjectManagementDbContext context,
        ILogger<ProjectManagementWriteRepository<T>> logger
        ) : IProjectManagementWriteRepository<T> where T : BaseEntity
    {
        private const int MaxTaskTitleLength = 200;
        private const int MaxTaskDescriptionLength = 5000;
        private const int MinPriority = 1;
        private const int MaxPriority = 5;

        public async Task AddTask(T entity)
        {
            if (entity == null)
            {
                logger.LogError("Eklenmeye çalışılan task ile ilgili entity null");
                throw new ArgumentNullException(nameof(entity), "Entity null olamaz");
            }

            try
            {
                ValidateEntity(entity);

                await context.Set<T>().AddAsync(entity);
                await context.SaveChangesAsync();

                logger.LogInformation("Entity başarıyla eklendi. Id: {EntityId}, Tip: {EntityType}",
                    entity.Id, typeof(T).Name);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Veritabanına kayıt eklenirken hata oluştu. Entity Id: {EntityId}",
                    entity.Id);
                throw new InvalidOperationException("Veritabanına kayıt eklenirken bir hata oluştu", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Entity eklenirken beklenmeyen hata. Entity Id: {EntityId}",
                    entity.Id);
                throw;
            }
        }

        public async Task DeleteTask(T task)
        {
            if (task == null)
            {
                logger.LogError("Silinmeye çalışılan task null");
                throw new ArgumentNullException(nameof(task), "Task null olamaz");
            }

            try
            {
                var existingEntity = await context.Set<T>()
                    .FirstOrDefaultAsync(e => e.Id == task.Id);

                if (existingEntity == null)
                {
                    logger.LogWarning("Silinmek istenen task bulunamadı. Id: {TaskId}", task.Id);
                    throw new InvalidOperationException($"Id'si {task.Id} olan kayıt bulunamadı");
                }

                context.Set<T>().Remove(existingEntity);
                await context.SaveChangesAsync();

                logger.LogInformation("Task başarıyla silindi. Id: {TaskId}, Tip: {EntityType}",
                    task.Id, typeof(T).Name);
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

        public async Task UpdateTask(T task)
        {
            if (task == null)
            {
                logger.LogError("Güncellenmeye çalışılan task null");
                throw new ArgumentNullException(nameof(task), "Task null olamaz");
            }

            try
            {
                ValidateEntity(task);

                var existingEntity = await context.Set<T>()
                    .FirstOrDefaultAsync(e => e.Id == task.Id);

                if (existingEntity == null)
                {
                    logger.LogWarning("Güncellenmek istenen task bulunamadı. Id: {TaskId}", task.Id);
                    throw new InvalidOperationException($"Id'si {task.Id} olan kayıt bulunamadı");
                }

                context.Entry(existingEntity).CurrentValues.SetValues(task);
                await context.SaveChangesAsync();

                logger.LogInformation("Task başarıyla güncellendi. Id: {TaskId}, Tip: {EntityType}",
                    task.Id, typeof(T).Name);
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

        private void ValidateEntity(T entity)
        {
            if (entity.Id == Guid.Empty)
            {
                logger.LogError("Entity Id boş olamaz");
                throw new ArgumentException("Entity Id boş olamaz", nameof(entity));
            }

            var properties = typeof(T).GetProperties();

            var titleProperty = properties.FirstOrDefault(p =>
                p.Name.Equals("Title", StringComparison.OrdinalIgnoreCase));

            if (titleProperty != null && titleProperty.PropertyType == typeof(string))
            {
                var title = titleProperty.GetValue(entity) as string;
                if (string.IsNullOrWhiteSpace(title))
                {
                    logger.LogError("Task başlığı boş olamaz. Entity Id: {EntityId}", entity.Id);
                    throw new ArgumentException("Task başlığı boş olamaz", nameof(entity));
                }

                if (title.Length > MaxTaskTitleLength)
                {
                    logger.LogError("Task başlığı maksimum {MaxLength} karakter olabilir. Mevcut: {CurrentLength}. Entity Id: {EntityId}",
                        MaxTaskTitleLength, title.Length, entity.Id);
                    throw new ArgumentException($"Task başlığı maksimum {MaxTaskTitleLength} karakter olabilir", nameof(entity));
                }
            }

            var descriptionProperty = properties.FirstOrDefault(p =>
                p.Name.Equals("Description", StringComparison.OrdinalIgnoreCase));

            if (descriptionProperty != null && descriptionProperty.PropertyType == typeof(string))
            {
                var description = descriptionProperty.GetValue(entity) as string;
                if (!string.IsNullOrWhiteSpace(description) && description.Length > MaxTaskDescriptionLength)
                {
                    logger.LogError("Task açıklaması maksimum {MaxLength} karakter olabilir. Mevcut: {CurrentLength}. Entity Id: {EntityId}",
                        MaxTaskDescriptionLength, description.Length, entity.Id);
                    throw new ArgumentException($"Task açıklaması maksimum {MaxTaskDescriptionLength} karakter olabilir", nameof(entity));
                }
            }

            var priorityProperty = properties.FirstOrDefault(p =>
                p.Name.Equals("Priority", StringComparison.OrdinalIgnoreCase));

            if (priorityProperty != null && priorityProperty.PropertyType == typeof(int))
            {
                var priority = (int)priorityProperty.GetValue(entity);
                if (priority < MinPriority || priority > MaxPriority)
                {
                    logger.LogError("Priority değeri {MinPriority} ile {MaxPriority} arasında olmalıdır. Mevcut: {CurrentPriority}. Entity Id: {EntityId}",
                        MinPriority, MaxPriority, priority, entity.Id);
                    throw new ArgumentException($"Priority değeri {MinPriority} ile {MaxPriority} arasında olmalıdır", nameof(entity));
                }
            }
        }
    }
}