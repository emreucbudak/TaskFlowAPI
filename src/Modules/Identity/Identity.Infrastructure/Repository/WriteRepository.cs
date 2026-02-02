using Identity.Application.Repositories;
using Identity.Infrastructure.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Infrastructure.Repository
{
    public class WriteRepository<T>(
        IdentityManagementDbContext context,
        ILogger<WriteRepository<T>> logger) : IWriteRepository<T>
        where T : class
    {
        private DbSet<T> db => context.Set<T>();

        public void Add(T entity)
        {
            ValidateEntity(entity, "ekleme");

            SetCreationMetadata(entity);

            try
            {
                logger.LogInformation(
                    "Yeni kayıt ekleniyor - Varlık: {EntityType}, Zaman: {Timestamp}",
                    typeof(T).Name, DateTime.UtcNow);

                db.Add(entity);

                logger.LogDebug(
                    "Kayıt context'e başarıyla eklendi - Varlık: {EntityType}",
                    typeof(T).Name);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Veritabanı kısıtlama hatası - Varlık: {EntityType}, Hata: {Message}",
                    typeof(T).Name, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} ekleme işlemi veritabanı kısıtlamalarını ihlal ediyor. " +
                    "Lütfen benzersiz alanları ve gerekli ilişkileri kontrol edin. " +
                    $"Detay: {ex.InnerException?.Message ?? ex.Message}",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen ekleme hatası - Varlık: {EntityType}",
                    typeof(T).Name);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlığı eklenirken beklenmeyen bir hata oluştu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }

        public void Update(T entity)
        {
            ValidateEntity(entity, "güncelleme");


            SetUpdateMetadata(entity);
            HandleConcurrencyToken(entity);

            try
            {
                logger.LogInformation(
                    "Kayıt güncelleniyor - Varlık: {EntityType}, Zaman: {Timestamp}",
                    typeof(T).Name, DateTime.UtcNow);

                db.Update(entity);

                logger.LogDebug(
                    "Kayıt başarıyla güncellendi - Varlık: {EntityType}",
                    typeof(T).Name);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogWarning(ex,
                    "Eşzamanlılık hatası - Varlık: {EntityType}, Zaman: {Timestamp}",
                    typeof(T).Name,DateTime.UtcNow);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kaydı başka bir kullanıcı tarafından değiştirilmiş. " +
                    "Lütfen sayfayı yenileyip güncel veriyi yükledikten sonra tekrar deneyin.",
                    ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Veritabanı kısıtlama hatası - Varlık: {EntityType}, Hata: {Message}",
                    typeof(T).Name, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} güncelleme işlemi veritabanı kısıtlamalarını ihlal ediyor. " +
                    "Lütfen benzersiz alanları ve ilişkileri kontrol edin. " +
                    $"Detay: {ex.InnerException?.Message ?? ex.Message}",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen güncelleme hatası - Varlık: {EntityType}",
                    typeof(T).Name);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlığı güncellenirken beklenmeyen bir hata oluştu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }


        public void Delete(T entity)
        {
            ValidateEntity(entity, "silme");
            try
            {
                if (entity is ISoftDelete softDelete)
                {
                    PerformSoftDelete(entity, softDelete);
                }
                else
                {
                    PerformHardDelete(entity);
                }
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Veritabanı kısıtlama hatası - Varlık: {EntityType}, İşlem: Silme, Hata: {Message}",
                    typeof(T).Name, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} silme işlemi başarısız. " +
                    "Bu kayıt başka kayıtlar tarafından kullanılıyor olabilir. " +
                    "Lütfen bağımlı kayıtları kontrol edin. " +
                    $"Detay: {ex.InnerException?.Message ?? ex.Message}",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen silme hatası - Varlık: {EntityType}",
                    typeof(T).Name);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlığı silinirken beklenmeyen bir hata oluştu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }


        public void PermanentDelete(T entity)
        {
            ValidateEntity(entity, "kalıcı silme");

            var entityId = entity is BaseEntity<Guid> baseEntity ? baseEntity.Id.ToString() : "N/A";

            try
            {
                logger.LogCritical(
                    "KALICI SİLME İŞLEMİ BAŞLATILDI - Varlık: {EntityType}, ID: {EntityId} Zaman: {Timestamp}",
                    typeof(T).Name, entityId, DateTime.UtcNow);

                db.Remove(entity);

                logger.LogWarning(
                    "Varlık veritabanından kalıcı olarak kaldırıldı - Varlık: {EntityType}, ID: {EntityId}",
                    typeof(T).Name, entityId);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Kalıcı silme kısıtlama hatası - Varlık: {EntityType}, ID: {EntityId}, Hata: {Message}",
                    typeof(T).Name, entityId, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kalıcı silme işlemi başarısız. " +
                    "Bu kayıt başka kayıtlar tarafından kullanılıyor ve silinemez. " +
                    "Lütfen önce bağımlı kayıtları silin. " +
                    $"Detay: {ex.InnerException?.Message ?? ex.Message}",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen kalıcı silme hatası - Varlık: {EntityType}, ID: {EntityId}",
                    typeof(T).Name, entityId);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlığı kalıcı silinirken beklenmeyen bir hata oluştu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }


        private void SetCreationMetadata(T entity)
        {
            if (entity is ICreatableEntity creatable)
            {
                creatable.CreatedAt = DateTime.UtcNow;
                creatable.CreatedBy = currentUserService.UserId;

                logger.LogDebug(
                    "Oluşturma bilgileri ayarlandı - Varlık: {EntityType}, Oluşturan: {CreatedBy}, Zaman: {CreatedAt}",
                    typeof(T).Name, currentUserService.UserId, DateTime.UtcNow);
            }
        }


        private void SetUpdateMetadata(T entity)
        {
            if (entity is IUpdatableEntity updatable)
            {
                updatable.UpdatedAt = DateTime.UtcNow;
                updatable.UpdatedBy = currentUserService.UserId;

                logger.LogDebug(
                    "Güncelleme bilgileri ayarlandı - Varlık: {EntityType}, Güncelleyen: {UpdatedBy}, Zaman: {UpdatedAt}",
                    typeof(T).Name, currentUserService.UserId, DateTime.UtcNow);
            }
        }


        private void HandleConcurrencyToken(T entity)
        {
            if (entity is IHasConcurrencyToken concurrency)
            {
                if (concurrency.RowVersion == null || concurrency.RowVersion.Length == 0)
                {
                    logger.LogError(
                        "Geçersiz concurrency token - Varlık: {EntityType}, Kullanıcı: {UserId}",
                        typeof(T).Name, currentUserService.UserId);
                    throw new InvalidOperationException(
                        $"{typeof(T).Name} için concurrency token geçersiz veya eksik. " +
                        "Lütfen kaydı yeniden yükleyip tekrar deneyin. " +
                        "Bu hata genellikle eski veri ile çalıştığınızda oluşur.");
                }

                var entry = context.Entry(entity);
                entry.Property(nameof(IHasConcurrencyToken.RowVersion))
                    .OriginalValue = concurrency.RowVersion;

                logger.LogDebug(
                    "Concurrency token ayarlandı - Varlık: {EntityType}, Kullanıcı: {UserId}",
                    typeof(T).Name, currentUserService.UserId);
            }
        }



        private void PerformSoftDelete(T entity, ISoftDelete softDelete)
        {
            softDelete.IsDeleted = true;
            softDelete.DeletedAt = DateTime.UtcNow;

            db.Update(entity);

            logger.LogInformation(
                "Kayıt geçici silindi (soft delete) - Varlık: {EntityType}, Zaman: {DeletedAt}",
                typeof(T).Name, DateTime.UtcNow);
        }


        private void PerformHardDelete(T entity)
        {
            db.Remove(entity);

            logger.LogWarning(
                "Kayıt kalıcı silindi (hard delete) - Varlık: {EntityType}, Zaman: {Timestamp}, Uyarı: Geri alınamaz",
                typeof(T).Name, DateTime.UtcNow);
        }

 
        private void ValidateEntity(T entity, string operation)
        {
            if (entity == null)
            {
                logger.LogError(
                    "Null varlık hatası - İşlem: {Operation}, Varlık Tipi: {EntityType}",
                    operation, typeof(T).Name);
                throw new ArgumentNullException(
                    nameof(entity),
                    $"{typeof(T).Name} {operation} işlemi için varlık null olamaz. " +
                    "Lütfen geçerli bir varlık nesnesi sağlayın.");
            }

            if (entity is BaseEntity<Guid> baseEntity && operation != "ekleme")
            {
                if (baseEntity.Id == Guid.Empty)
                {
                    logger.LogError(
                        "Geçersiz ID hatası - İşlem: {Operation}, Varlık: {EntityType}, ID: {EntityId}",
                        operation, typeof(T).Name, baseEntity.Id);
                    throw new ArgumentException(
                        $"{typeof(T).Name} {operation} işlemi için varlık ID'si boş olamaz. " +
                        "Lütfen geçerli bir ID ile varlık sağlayın.",
                        nameof(entity));
                }
            }
        }
    }
}