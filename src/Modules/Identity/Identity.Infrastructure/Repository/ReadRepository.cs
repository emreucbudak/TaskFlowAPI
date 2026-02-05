using Identity.Application.Repositories;
using Identity.Infrastructure.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Infrastructure.Repository
{
    public class ReadRepository<T, TKey>(
        IdentityManagementDbContext context,
        ILogger<ReadRepository<T, TKey>> logger) : IReadRepository<T, TKey>
        where T : BaseEntity<TKey>
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;
        private const int MaxTotalRecords = 10000;

        private DbSet<T> db => context.Set<T>();


        public async Task<PagedResult<T>> GetAllAsync(int pageSize,
            int page = 1,
            bool trackChanges = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Kayıtlar getiriliyor - Varlık: {EntityType}, Sayfa: {Page}/{TotalPages}, Boyut: {PageSize}",
                typeof(T).Name, page, "?", pageSize);

            try
            {
                var query = trackChanges ? db.AsTracking() : db.AsNoTracking();

                if (inc != null)
                {
                    query = inc(query);
                }

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                if (totalCount > MaxTotalRecords)
                {
                    logger.LogWarning(
                        "Kayıt sayısı limiti aşıyor - Varlık: {EntityType}, Toplam: {TotalCount}, Limit: {MaxLimit}, Öneri: Filtreleme kullanın",
                        typeof(T).Name, totalCount, MaxTotalRecords);
                }

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                logger.LogInformation(
                    "Kayıtlar başarıyla getirildi - Varlık: {EntityType}, Getirilen: {ItemCount}/{TotalCount}, Sayfa: {Page}/{TotalPages}",
                    typeof(T).Name, items.Count, totalCount, page, totalPages);

                return new PagedResult<T>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex,
                    "Geçersiz sorgu işlemi - Varlık: {EntityType}, Sayfa: {Page}, Hata: {Message}",
                    typeof(T).Name, page, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kayıtları getirilirken geçersiz sorgu hatası oluştu. " +
                    "Lütfen include parametrelerini ve filtreleri kontrol edin.",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen hata - Varlık: {EntityType}, Sayfa: {Page}, Boyut: {PageSize}",
                    typeof(T).Name, page, pageSize);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kayıtları getirilirken beklenmeyen bir hata oluştu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }

        public async Task<T> GetByIdAsync(
            bool trackChanges,
            TKey id,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            ValidateId(id);

            logger.LogInformation(
                "ID ile kayıt getiriliyor - Varlık: {EntityType}, ID: {Id}, Takip: {TrackChanges}",
                typeof(T).Name, id, trackChanges);

            try
            {
                var query = trackChanges ? db.AsTracking() : db.AsNoTracking();

                if (inc != null)
                {
                    query = inc(query);
                }

                var result = await query.FirstOrDefaultAsync(x => x.Id!.Equals(id));

                if (result == null)
                {
                    logger.LogWarning(
                        "Kayıt bulunamadı - Varlık: {EntityType}, ID: {Id}",
                        typeof(T).Name, id);
                    return null;
                }

                logger.LogDebug(
                    "Kayıt başarıyla bulundu - Varlık: {EntityType}, ID: {Id}",
                    typeof(T).Name, id);
                return result;
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex,
                    "Geçersiz sorgu işlemi - Varlık: {EntityType}, ID: {Id}, Hata: {Message}",
                    typeof(T).Name, id, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kaydı (ID: {id}) getirilirken geçersiz sorgu hatası oluştu. " +
                    "Lütfen include parametrelerini kontrol edin.",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen hata - Varlık: {EntityType}, ID: {Id}",
                    typeof(T).Name, id);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kaydı (ID: {id}) getirilirken beklenmeyen bir hata oluştu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }

        private void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1)
            {
                logger.LogWarning(
                    "Geçersiz sayfa numarası düzeltildi - İstenen: {RequestedPage}, Kullanılan: 1",
                    page);
                page = 1;
            }

            if (pageSize < 1)
            {
                logger.LogWarning(
                    "Geçersiz sayfa boyutu düzeltildi - İstenen: {RequestedSize}, Kullanılan: {DefaultSize}",
                    pageSize, DefaultPageSize);
                pageSize = DefaultPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning(
                    "Sayfa boyutu sınırlandırıldı - İstenen: {RequestedSize}, Kullanılan: {MaxSize}, Maksimum: {MaxPageSize}",
                    pageSize, MaxPageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }
        }

        private void ValidateId(TKey id)
        {
            if (id == null)
            {
                logger.LogError(
                    "Null ID parametresi - Varlık: {EntityType}",
                    typeof(T).Name);
                throw new ArgumentNullException(
                    nameof(id),
                    $"{typeof(T).Name} için ID parametresi null olamaz.");
            }

            if (id.Equals(default(TKey)))
            {
                logger.LogError(
                    "Varsayılan ID değeri - Varlık: {EntityType}, ID Tipi: {KeyType}",
                    typeof(T).Name, typeof(TKey).Name);
                throw new ArgumentException(
                    $"{typeof(T).Name} için geçersiz ID değeri sağlandı. ID varsayılan değer olamaz.",
                    nameof(id));
            }
        }
    }
}