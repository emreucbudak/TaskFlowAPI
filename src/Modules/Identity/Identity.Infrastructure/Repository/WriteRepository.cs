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

        public async Task AddAsync(T entity)
        {
            ValidateEntity(entity);
            try
            {
                logger.LogInformation(
                    "Yeni kayıt ekleniyor - Varlık: {EntityType}, Zaman: {Timestamp}",
                    typeof(T).Name, DateTime.UtcNow);

                await db.AddAsync(entity);

                logger.LogInformation(
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

        public void UpdateAsync(T entity)
        {
            ValidateEntity(entity);
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


        public void DeleteAsync(T entity)
        {
            // Varlık doğrulama
            ValidateEntity(entity);

            try
            {
                db.Remove(entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogWarning(ex,
                    "Eşzamanlılık hatası - Varlık: {EntityType}, Hata: {Message}",
                    typeof(T).Name, ex.Message);

                throw new InvalidOperationException(
                    $"{typeof(T).Name} silme işlemi başarısız. Bu kayıt başka bir kullanıcı tarafından değiştirilmiş veya silinmiş olabilir.",
                    ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Veritabanı kısıtlama hatası - Varlık: {EntityType}, İşlem: Silme",
                    typeof(T).Name);

                throw new InvalidOperationException(
                    $"{typeof(T).Name} silme işlemi başarısız. Bu kayıt başka kayıtlar tarafından kullanılıyor olabilir.",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen silme hatası - Varlık: {EntityType}",
                    typeof(T).Name);

                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlığı silinirken beklenmeyen bir hata oluştu.",
                    ex);
            }
        }
        private void ValidateEntity(T entity)
        {
            if (entity == null)
            {
                logger.LogError(
                    "Null varlık hatası , Varlık Tipi: {EntityType}",
                    typeof(T).Name);
                throw new ArgumentNullException(
                    nameof(entity),
                    $"{typeof(T).Name} işlemi için varlık null olamaz. " +
                    "Lütfen geçerli bir varlık nesnesi sağlayın.");
            }
        }
    }
}