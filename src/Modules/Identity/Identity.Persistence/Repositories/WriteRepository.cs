using Identity.Application.Repositories;
using Identity.Persistence.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Persistence.Repositories
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
                    "Yeni kayýt ekleniyor - Varlýk: {EntityType}, Zaman: {Timestamp}",
                    typeof(T).Name, DateTime.UtcNow);

                await db.AddAsync(entity);

                logger.LogInformation(
                    "Kayýt context'e baþarýyla eklendi - Varlýk: {EntityType}",
                    typeof(T).Name);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Veritabaný kýsýtlama hatasý - Varlýk: {EntityType}, Hata: {Message}",
                    typeof(T).Name, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} ekleme iþlemi veritabaný kýsýtlamalarýný ihlal ediyor. " +
                    "Lütfen benzersiz alanlarý ve gerekli iliþkileri kontrol edin. " +
                    $"Detay: {ex.InnerException?.Message ?? ex.Message}",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen ekleme hatasý - Varlýk: {EntityType}",
                    typeof(T).Name);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlýðý eklenirken beklenmeyen bir hata oluþtu. " +
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
                    "Kayýt güncelleniyor - Varlýk: {EntityType}, Zaman: {Timestamp}",
                    typeof(T).Name, DateTime.UtcNow);

                db.Update(entity);

                logger.LogDebug(
                    "Kayýt baþarýyla güncellendi - Varlýk: {EntityType}",
                    typeof(T).Name);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogWarning(ex,
                    "Eþzamanlýlýk hatasý - Varlýk: {EntityType}, Zaman: {Timestamp}",
                    typeof(T).Name,DateTime.UtcNow);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kaydý baþka bir kullanýcý tarafýndan deðiþtirilmiþ. " +
                    "Lütfen sayfayý yenileyip güncel veriyi yükledikten sonra tekrar deneyin.",
                    ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Veritabaný kýsýtlama hatasý - Varlýk: {EntityType}, Hata: {Message}",
                    typeof(T).Name, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} güncelleme iþlemi veritabaný kýsýtlamalarýný ihlal ediyor. " +
                    "Lütfen benzersiz alanlarý ve iliþkileri kontrol edin. " +
                    $"Detay: {ex.InnerException?.Message ?? ex.Message}",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen güncelleme hatasý - Varlýk: {EntityType}",
                    typeof(T).Name);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlýðý güncellenirken beklenmeyen bir hata oluþtu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }


        public void DeleteAsync(T entity)
        {
            // Varlýk doðrulama
            ValidateEntity(entity);

            try
            {
                db.Remove(entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogWarning(ex,
                    "Eþzamanlýlýk hatasý - Varlýk: {EntityType}, Hata: {Message}",
                    typeof(T).Name, ex.Message);

                throw new InvalidOperationException(
                    $"{typeof(T).Name} silme iþlemi baþarýsýz. Bu kayýt baþka bir kullanýcý tarafýndan deðiþtirilmiþ veya silinmiþ olabilir.",
                    ex);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Veritabaný kýsýtlama hatasý - Varlýk: {EntityType}, Ýþlem: Silme",
                    typeof(T).Name);

                throw new InvalidOperationException(
                    $"{typeof(T).Name} silme iþlemi baþarýsýz. Bu kayýt baþka kayýtlar tarafýndan kullanýlýyor olabilir.",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen silme hatasý - Varlýk: {EntityType}",
                    typeof(T).Name);

                throw new InvalidOperationException(
                    $"{typeof(T).Name} varlýðý silinirken beklenmeyen bir hata oluþtu.",
                    ex);
            }
        }
        private void ValidateEntity(T entity)
        {
            if (entity == null)
            {
                logger.LogError(
                    "Null varlýk hatasý , Varlýk Tipi: {EntityType}",
                    typeof(T).Name);
                throw new ArgumentNullException(
                    nameof(entity),
                    $"{typeof(T).Name} iþlemi için varlýk null olamaz. " +
                    "Lütfen geçerli bir varlýk nesnesi saðlayýn.");
            }
        }
    }
}
