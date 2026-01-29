using Identity.Application.Repositories;
using Identity.Infrastructure.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Infrastructure.Repository
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        Guid? DeletedBy { get; set; }
    }

    public interface IUserOwnedEntity
    {
        Guid UserId { get; set; }
    }

    public interface IHasConcurrencyToken
    {
        byte[] RowVersion { get; set; }
    }

    public interface ICurrentUserService
    {
        Guid UserId { get; }
        bool IsAuthenticated { get; }
    }

    public interface ICreatableEntity
    {
        DateTime CreatedAt { get; set; }
        Guid CreatedBy { get; set; }
    }

    public interface IUpdatableEntity
    {
        DateTime? UpdatedAt { get; set; }
        Guid? UpdatedBy { get; set; }
    }

    public class WriteRepository<T>(
        IdentityManagementDbContext context,
        ILogger<WriteRepository<T>> logger,
        ICurrentUserService currentUserService) : IWriteRepository<T>
        where T : class
    {
        private DbSet<T> db => context.Set<T>();

        public void Add(T entity)
        {
            ValidateEntity(entity, "Ekleme");
            ValidateUserAuthentication();
            ValidateAddPermission(entity);

            if (entity is ICreatableEntity creatable)
            {
                creatable.CreatedAt = DateTime.UtcNow;
                creatable.CreatedBy = currentUserService.UserId;
            }

            try
            {
                logger.LogInformation(
                    "Yeni kayıt ekleniyor - Tür: {Type}",
                    typeof(T).Name);

                db.Add(entity);

                logger.LogDebug(
                    "Kayıt context'e eklendi - Tür: {Type}",
                    typeof(T).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kayıt eklenirken hata oluştu - Tür: {Type}", typeof(T).Name);
                throw;
            }
        }

        public void Update(T entity)
        {
            ValidateEntity(entity, "Güncelleme");
            ValidateUserAuthentication();
            ValidateUpdatePermission(entity);

            if (entity is IUpdatableEntity updatable)
            {
                updatable.UpdatedAt = DateTime.UtcNow;
                updatable.UpdatedBy = currentUserService.UserId;
            }

            if (entity is IHasConcurrencyToken concurrency)
            {
                var entry = context.Entry(entity);
                if (concurrency.RowVersion == null || concurrency.RowVersion.Length == 0)
                {
                    logger.LogError("Geçersiz concurrency token - Tür: {Type}", typeof(T).Name);
                    throw new InvalidOperationException("Concurrency token geçersiz veya eksik");
                }

                entry.Property(nameof(IHasConcurrencyToken.RowVersion))
                    .OriginalValue = concurrency.RowVersion;

                logger.LogDebug("Concurrency token ayarlandı - Tür: {Type}", typeof(T).Name);
            }

            try
            {
                logger.LogInformation("Kayıt güncelleniyor - Tür: {Type}", typeof(T).Name);
                db.Update(entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogWarning(ex, "Eşzamanlılık hatası - Tür: {Type}", typeof(T).Name);
                throw new InvalidOperationException("Kayıt başka bir kullanıcı tarafından değiştirilmiş", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kayıt güncellenirken hata oluştu - Tür: {Type}", typeof(T).Name);
                throw;
            }
        }

        public void Delete(T entity)
        {
            ValidateEntity(entity, "Silme");
            ValidateUserAuthentication();
            ValidateDeletePermission(entity);

            try
            {
                if (entity is ISoftDelete softDelete)
                {
                    softDelete.IsDeleted = true;
                    softDelete.DeletedAt = DateTime.UtcNow;
                    softDelete.DeletedBy = currentUserService.UserId;

                    db.Update(entity);

                    logger.LogInformation("Kayıt geçici olarak silindi - Tür: {Type}", typeof(T).Name);
                }
                else
                {
                    db.Remove(entity);

                    logger.LogWarning("Kayıt kalıcı olarak silindi - Tür: {Type}", typeof(T).Name);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kayıt silinirken hata oluştu - Tür: {Type}", typeof(T).Name);
                throw;
            }
        }

        public void PermanentDelete(T entity)
        {
            ValidateEntity(entity, "Kalıcı Silme");
            ValidateUserAuthentication();
            ValidateDeletePermission(entity);

            try
            {
                logger.LogCritical("KALICI SİLME İŞLEMİ - Tür: {Type}", typeof(T).Name);

                db.Remove(entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kalıcı silme işleminde hata - Tür: {Type}", typeof(T).Name);
                throw;
            }
        }

        private void ValidateEntity(T entity, string operation)
        {
            if (entity == null)
            {
                logger.LogError("{Operation} işlemi için null entity sağlandı", operation);
                throw new ArgumentNullException(nameof(entity), $"{operation} işlemi için entity null olamaz");
            }

            if (entity is BaseEntity<Guid> baseEntity)
            {
                if (baseEntity.Id == Guid.Empty && operation != "Ekleme")
                {
                    logger.LogError("{Operation} işlemi için geçersiz ID - Tür: {Type}", operation, typeof(T).Name);
                    throw new ArgumentException($"{operation} işlemi için entity ID boş olamaz", nameof(entity));
                }
            }
        }

        private void ValidateUserAuthentication()
        {
            if (!currentUserService.IsAuthenticated)
            {
                logger.LogError("Kimlik doğrulaması yapılmamış kullanıcı erişim denemesi");
                throw new UnauthorizedAccessException("Kullanıcı kimlik doğrulaması gerekli");
            }

            if (currentUserService.UserId == Guid.Empty)
            {
                logger.LogError("Geçersiz kullanıcı ID'si");
                throw new UnauthorizedAccessException("Geçersiz kullanıcı kimliği");
            }
        }

        private void ValidateAddPermission(T entity)
        {
            if (entity is IUserOwnedEntity userOwned)
            {
                if (userOwned.UserId == Guid.Empty)
                {
                    userOwned.UserId = currentUserService.UserId;
                    logger.LogDebug("Kullanıcı ID'si mevcut kullanıcıya atandı - Tür: {Type}", typeof(T).Name);
                }
                else if (userOwned.UserId != currentUserService.UserId)
                {
                    logger.LogWarning(
                        "Yetkisiz ekleme denemesi - Tür: {Type}",
                        typeof(T).Name);

                    throw new UnauthorizedAccessException("Başka kullanıcılar için kayıt ekleyemezsiniz");
                }
            }
        }

        private void ValidateUpdatePermission(T entity)
        {
            if (entity is IUserOwnedEntity userOwned)
            {
                if (userOwned.UserId == Guid.Empty)
                {
                    logger.LogError("Güncelleme işlemi için geçersiz kullanıcı ID'si - Tür: {Type}", typeof(T).Name);
                    throw new InvalidOperationException("Entity'nin kullanıcı ID'si geçersiz");
                }

                if (userOwned.UserId != currentUserService.UserId)
                {
                    logger.LogWarning(
                        "Yetkisiz güncelleme denemesi - Tür: {Type}",
                        typeof(T).Name);

                    throw new UnauthorizedAccessException("Size ait olmayan kayıtları güncelleyemezsiniz");
                }
            }
        }

        private void ValidateDeletePermission(T entity)
        {
            if (entity is IUserOwnedEntity userOwned)
            {
                if (userOwned.UserId == Guid.Empty)
                {
                    logger.LogError("Silme işlemi için geçersiz kullanıcı ID'si - Tür: {Type}", typeof(T).Name);
                    throw new InvalidOperationException("Entity'nin kullanıcı ID'si geçersiz");
                }

                if (userOwned.UserId != currentUserService.UserId)
                {
                    logger.LogWarning(
                        "Yetkisiz silme denemesi - Tür: {Type}",
                        typeof(T).Name);

                    throw new UnauthorizedAccessException("Size ait olmayan kayıtları silemezsiniz");
                }
            }
        }
    }
}