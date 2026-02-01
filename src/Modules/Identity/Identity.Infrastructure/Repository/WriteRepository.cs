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

    /// <summary>
    /// Generic repository sınıfı - varlıklar için yazma işlemlerini gerçekleştirir.
    /// Ekleme, güncelleme, silme işlemlerinde güvenlik, denetim ve concurrency kontrolü sağlar.
    /// </summary>
    /// <typeparam name="T">Varlık tipi</typeparam>
    public class WriteRepository<T>(
        IdentityManagementDbContext context,
        ILogger<WriteRepository<T>> logger,
        ICurrentUserService currentUserService) : IWriteRepository<T>
        where T : class
    {
        private DbSet<T> db => context.Set<T>();

        /// <summary>
        /// Yeni bir varlık ekler ve otomatik olarak oluşturma bilgilerini doldurur.
        /// </summary>
        /// <param name="entity">Eklenecek varlık</param>
        /// <exception cref="ArgumentNullException">Varlık null ise</exception>
        /// <exception cref="UnauthorizedAccessException">Kullanıcı yetkisiz ise</exception>
        public void Add(T entity)
        {
            ValidateEntity(entity, "ekleme");
            ValidateUserAuthentication("ekleme");
            ValidateAddPermission(entity);

            SetCreationMetadata(entity);

            try
            {
                logger.LogInformation(
                    "Yeni kayıt ekleniyor - Varlık: {EntityType}, Kullanıcı: {UserId}, Zaman: {Timestamp}",
                    typeof(T).Name, currentUserService.UserId, DateTime.UtcNow);

                db.Add(entity);

                logger.LogDebug(
                    "Kayıt context'e başarıyla eklendi - Varlık: {EntityType}, Kullanıcı: {UserId}",
                    typeof(T).Name, currentUserService.UserId);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Veritabanı kısıtlama hatası - Varlık: {EntityType}, Kullanıcı: {UserId}, Hata: {Message}",
                    typeof(T).Name, currentUserService.UserId, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} ekleme işlemi veritabanı kısıtlamalarını ihlal ediyor. " +
                    "Lütfen benzersiz alanları ve gerekli ilişkileri kontrol edin. " +
                    $"Detay: {ex.InnerException?.Message ?? ex.Message}",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen ekleme hatası - Varlık: {EntityType}, Kullanıcı: {UserId}",
                    typeof(T).Name, currentUserService.UserId);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlığı eklenirken beklenmeyen bir hata oluştu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }

        /// <summary>
        /// Mevcut bir varlığı günceller ve otomatik olarak güncelleme bilgilerini doldurur.
        /// Concurrency token kontrolü yapar.
        /// </summary>
        /// <param name="entity">Güncellenecek varlık</param>
        /// <exception cref="ArgumentNullException">Varlık null ise</exception>
        /// <exception cref="UnauthorizedAccessException">Kullanıcı yetkisiz ise</exception>
        /// <exception cref="InvalidOperationException">Concurrency hatası veya geçersiz durum</exception>
        public void Update(T entity)
        {
            ValidateEntity(entity, "güncelleme");
            ValidateUserAuthentication("güncelleme");
            ValidateUpdatePermission(entity);

            SetUpdateMetadata(entity);
            HandleConcurrencyToken(entity);

            try
            {
                logger.LogInformation(
                    "Kayıt güncelleniyor - Varlık: {EntityType}, Kullanıcı: {UserId}, Zaman: {Timestamp}",
                    typeof(T).Name, currentUserService.UserId, DateTime.UtcNow);

                db.Update(entity);

                logger.LogDebug(
                    "Kayıt başarıyla güncellendi - Varlık: {EntityType}, Kullanıcı: {UserId}",
                    typeof(T).Name, currentUserService.UserId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogWarning(ex,
                    "Eşzamanlılık hatası - Varlık: {EntityType}, Kullanıcı: {UserId}, Zaman: {Timestamp}",
                    typeof(T).Name, currentUserService.UserId, DateTime.UtcNow);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kaydı başka bir kullanıcı tarafından değiştirilmiş. " +
                    "Lütfen sayfayı yenileyip güncel veriyi yükledikten sonra tekrar deneyin.",
                    ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Veritabanı kısıtlama hatası - Varlık: {EntityType}, Kullanıcı: {UserId}, Hata: {Message}",
                    typeof(T).Name, currentUserService.UserId, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} güncelleme işlemi veritabanı kısıtlamalarını ihlal ediyor. " +
                    "Lütfen benzersiz alanları ve ilişkileri kontrol edin. " +
                    $"Detay: {ex.InnerException?.Message ?? ex.Message}",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen güncelleme hatası - Varlık: {EntityType}, Kullanıcı: {UserId}",
                    typeof(T).Name, currentUserService.UserId);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlığı güncellenirken beklenmeyen bir hata oluştu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }

        /// <summary>
        /// Varlığı siler. ISoftDelete uyguluyorsa geçici, uygulamıyorsa kalıcı siler.
        /// </summary>
        /// <param name="entity">Silinecek varlık</param>
        /// <exception cref="ArgumentNullException">Varlık null ise</exception>
        /// <exception cref="UnauthorizedAccessException">Kullanıcı yetkisiz ise</exception>
        public void Delete(T entity)
        {
            ValidateEntity(entity, "silme");
            ValidateUserAuthentication("silme");
            ValidateDeletePermission(entity);

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
                    "Veritabanı kısıtlama hatası - Varlık: {EntityType}, İşlem: Silme, Kullanıcı: {UserId}, Hata: {Message}",
                    typeof(T).Name, currentUserService.UserId, ex.Message);
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
                    "Beklenmeyen silme hatası - Varlık: {EntityType}, Kullanıcı: {UserId}",
                    typeof(T).Name, currentUserService.UserId);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlığı silinirken beklenmeyen bir hata oluştu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }

        /// <summary>
        /// Varlığı veritabanından kalıcı olarak siler (geri alınamaz).
        /// Bu işlem kritik loglama gerektirir ve dikkatli kullanılmalıdır.
        /// </summary>
        /// <param name="entity">Kalıcı silinecek varlık</param>
        /// <exception cref="ArgumentNullException">Varlık null ise</exception>
        /// <exception cref="UnauthorizedAccessException">Kullanıcı yetkisiz ise</exception>
        public void PermanentDelete(T entity)
        {
            ValidateEntity(entity, "kalıcı silme");
            ValidateUserAuthentication("kalıcı silme");
            ValidateDeletePermission(entity);

            var entityId = entity is BaseEntity<Guid> baseEntity ? baseEntity.Id.ToString() : "N/A";

            try
            {
                logger.LogCritical(
                    "KALICI SİLME İŞLEMİ BAŞLATILDI - Varlık: {EntityType}, ID: {EntityId}, Kullanıcı: {UserId}, Zaman: {Timestamp}",
                    typeof(T).Name, entityId, currentUserService.UserId, DateTime.UtcNow);

                db.Remove(entity);

                logger.LogWarning(
                    "Varlık veritabanından kalıcı olarak kaldırıldı - Varlık: {EntityType}, ID: {EntityId}, Kullanıcı: {UserId}",
                    typeof(T).Name, entityId, currentUserService.UserId);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Kalıcı silme kısıtlama hatası - Varlık: {EntityType}, ID: {EntityId}, Kullanıcı: {UserId}, Hata: {Message}",
                    typeof(T).Name, entityId, currentUserService.UserId, ex.Message);
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
                    "Beklenmeyen kalıcı silme hatası - Varlık: {EntityType}, ID: {EntityId}, Kullanıcı: {UserId}",
                    typeof(T).Name, entityId, currentUserService.UserId);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlığı kalıcı silinirken beklenmeyen bir hata oluştu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }

        /// <summary>
        /// Oluşturma metadata'sını (CreatedAt, CreatedBy) ayarlar.
        /// </summary>
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

        /// <summary>
        /// Güncelleme metadata'sını (UpdatedAt, UpdatedBy) ayarlar.
        /// </summary>
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

        /// <summary>
        /// Concurrency token kontrolü yapar ve ayarlar.
        /// </summary>
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

        /// <summary>
        /// Soft delete (geçici silme) işlemini gerçekleştirir.
        /// </summary>
        private void PerformSoftDelete(T entity, ISoftDelete softDelete)
        {
            softDelete.IsDeleted = true;
            softDelete.DeletedAt = DateTime.UtcNow;
            softDelete.DeletedBy = currentUserService.UserId;

            db.Update(entity);

            logger.LogInformation(
                "Kayıt geçici silindi (soft delete) - Varlık: {EntityType}, Silen: {DeletedBy}, Zaman: {DeletedAt}",
                typeof(T).Name, currentUserService.UserId, DateTime.UtcNow);
        }

        /// <summary>
        /// Hard delete (kalıcı silme) işlemini gerçekleştirir.
        /// </summary>
        private void PerformHardDelete(T entity)
        {
            db.Remove(entity);

            logger.LogWarning(
                "Kayıt kalıcı silindi (hard delete) - Varlık: {EntityType}, Silen: {UserId}, Zaman: {Timestamp}, Uyarı: Geri alınamaz",
                typeof(T).Name, currentUserService.UserId, DateTime.UtcNow);
        }

        /// <summary>
        /// Varlık validasyonu yapar.
        /// </summary>
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

        /// <summary>
        /// Kullanıcı kimlik doğrulamasını kontrol eder.
        /// </summary>
        private void ValidateUserAuthentication(string operation)
        {
            if (!currentUserService.IsAuthenticated)
            {
                logger.LogError(
                    "Kimlik doğrulaması yapılmamış erişim denemesi - İşlem: {Operation}, Varlık: {EntityType}",
                    operation, typeof(T).Name);
                throw new UnauthorizedAccessException(
                    $"{typeof(T).Name} {operation} işlemi için kullanıcı kimlik doğrulaması gerekli. " +
                    "Lütfen giriş yapın.");
            }

            if (currentUserService.UserId == Guid.Empty)
            {
                logger.LogError(
                    "Geçersiz kullanıcı ID'si - İşlem: {Operation}, Varlık: {EntityType}, Kullanıcı ID: {UserId}",
                    operation, typeof(T).Name, currentUserService.UserId);
                throw new UnauthorizedAccessException(
                    "Geçersiz kullanıcı kimliği tespit edildi. " +
                    "Lütfen çıkış yapıp tekrar giriş yapın.");
            }
        }

        /// <summary>
        /// Ekleme yetkisi kontrolü yapar.
        /// </summary>
        private void ValidateAddPermission(T entity)
        {
            if (entity is not IUserOwnedEntity userOwned)
            {
                return;
            }

            if (userOwned.UserId == Guid.Empty)
            {
                userOwned.UserId = currentUserService.UserId;
                logger.LogDebug(
                    "Kullanıcı ID'si otomatik atandı - Varlık: {EntityType}, Atanan Kullanıcı: {AssignedUserId}",
                    typeof(T).Name, currentUserService.UserId);
                return;
            }

            if (userOwned.UserId != currentUserService.UserId)
            {
                logger.LogWarning(
                    "Yetkisiz ekleme denemesi - Varlık: {EntityType}, Hedef Kullanıcı: {TargetUserId}, Deneme Yapan: {CurrentUserId}, Zaman: {Timestamp}",
                    typeof(T).Name, userOwned.UserId, currentUserService.UserId, DateTime.UtcNow);
                throw new UnauthorizedAccessException(
                    $"{typeof(T).Name} ekleme yetkisi yok. " +
                    "Başka kullanıcılar adına kayıt ekleyemezsiniz. " +
                    "Sadece kendi kayıtlarınızı oluşturabilirsiniz.");
            }
        }

        /// <summary>
        /// Güncelleme yetkisi kontrolü yapar.
        /// </summary>
        private void ValidateUpdatePermission(T entity)
        {
            if (entity is not IUserOwnedEntity userOwned)
            {
                return;
            }

            if (userOwned.UserId == Guid.Empty)
            {
                logger.LogError(
                    "Boş kullanıcı ID'si hatası - Varlık: {EntityType}, İşlem: güncelleme",
                    typeof(T).Name);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} için kullanıcı ID'si geçersiz. " +
                    "Varlık verileri bozuk olabilir. Lütfen kaydı yeniden yükleyin.");
            }

            if (userOwned.UserId != currentUserService.UserId)
            {
                logger.LogWarning(
                    "Yetkisiz güncelleme denemesi - Varlık: {EntityType}, Kayıt Sahibi: {OwnerId}, Deneme Yapan: {CurrentUserId}, Zaman: {Timestamp}",
                    typeof(T).Name, userOwned.UserId, currentUserService.UserId, DateTime.UtcNow);
                throw new UnauthorizedAccessException(
                    $"{typeof(T).Name} güncelleme yetkisi yok. " +
                    "Size ait olmayan kayıtları güncelleyemezsiniz. " +
                    "Sadece kendi kayıtlarınızı düzenleyebilirsiniz.");
            }
        }

        /// <summary>
        /// Silme yetkisi kontrolü yapar.
        /// </summary>
        private void ValidateDeletePermission(T entity)
        {
            if (entity is not IUserOwnedEntity userOwned)
            {
                return;
            }

            if (userOwned.UserId == Guid.Empty)
            {
                logger.LogError(
                    "Boş kullanıcı ID'si hatası - Varlık: {EntityType}, İşlem: silme",
                    typeof(T).Name);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} için kullanıcı ID'si geçersiz. " +
                    "Varlık verileri bozuk olabilir. Lütfen kaydı yeniden yükleyin.");
            }

            if (userOwned.UserId != currentUserService.UserId)
            {
                logger.LogWarning(
                    "Yetkisiz silme denemesi - Varlık: {EntityType}, Kayıt Sahibi: {OwnerId}, Deneme Yapan: {CurrentUserId}, Zaman: {Timestamp}",
                    typeof(T).Name, userOwned.UserId, currentUserService.UserId, DateTime.UtcNow);
                throw new UnauthorizedAccessException(
                    $"{typeof(T).Name} silme yetkisi yok. " +
                    "Size ait olmayan kayıtları silemezsiniz. " +
                    "Sadece kendi kayıtlarınızı silebilirsiniz.");
            }
        }
    }
}