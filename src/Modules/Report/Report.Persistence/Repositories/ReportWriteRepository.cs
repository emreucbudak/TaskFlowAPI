using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Report.Application.Repositories;
using Report.Persistence.Data;

namespace Report.Persistence.Repositories
{
    public class ReportWriteRepository(
        ReportDbContext context,
        ILogger<ReportWriteRepository> logger) : IReportWriteRepository
    {
        private DbSet<Domain.Entities.Report> db => context.Reports;

        public async Task AddAsync(Domain.Entities.Report entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            try
            {
                logger.LogInformation("Yeni rapor ekleniyor. User: {UserId}", entity.UserId);
                await db.AddAsync(entity);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Rapor eklenirken veritabanı hatası oluştu.");
                throw new InvalidOperationException("Rapor kaydedilirken veritabanı kısıtlamalarına takıldı. Lütfen verileri kontrol edin.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Rapor eklenirken beklenmeyen hata.");
                throw new InvalidOperationException("Rapor ekleme işlemi başarısız oldu.", ex);
            }
        }

        public void UpdateAsync(Domain.Entities.Report entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            try
            {
                logger.LogInformation("Rapor güncelleniyor. ID: {Id}", entity.Id);
                db.Update(entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogWarning(ex, "Rapor güncellenirken eşzamanlılık hatası (Concurrency). ID: {Id}", entity.Id);
                throw new InvalidOperationException("Bu rapor başka bir kullanıcı tarafından güncellenmiş. Lütfen veriyi yenileyip tekrar deneyin.", ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Rapor güncellenirken hata. ID: {Id}", entity.Id);
                throw new InvalidOperationException("Rapor güncelleme işlemi başarısız.", ex);
            }
        }

        public void DeleteAsync(Domain.Entities.Report entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            try
            {
                logger.LogInformation("Rapor siliniyor. ID: {Id}", entity.Id);
                db.Remove(entity);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Rapor silinirken hata. ID: {Id}", entity.Id);
                throw new InvalidOperationException("Rapor silme işlemi başarısız.", ex);
            }
        }
    }
}
