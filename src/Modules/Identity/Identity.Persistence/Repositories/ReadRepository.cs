using Identity.Application.Repositories;
using Identity.Persistence.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Persistence.Repositories
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
                "Kayýtlar getiriliyor - Varlýk: {EntityType}, Sayfa: {Page}/{TotalPages}, Boyut: {PageSize}",
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
                        "Kayýt sayýsý limiti aþýyor - Varlýk: {EntityType}, Toplam: {TotalCount}, Limit: {MaxLimit}, Öneri: Filtreleme kullanýn",
                        typeof(T).Name, totalCount, MaxTotalRecords);
                }

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                logger.LogInformation(
                    "Kayýtlar baþarýyla getirildi - Varlýk: {EntityType}, Getirilen: {ItemCount}/{TotalCount}, Sayfa: {Page}/{TotalPages}",
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
                    "Geçersiz sorgu iþlemi - Varlýk: {EntityType}, Sayfa: {Page}, Hata: {Message}",
                    typeof(T).Name, page, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kayýtlarý getirilirken geçersiz sorgu hatasý oluþtu. " +
                    "Lütfen include parametrelerini ve filtreleri kontrol edin.",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen hata - Varlýk: {EntityType}, Sayfa: {Page}, Boyut: {PageSize}",
                    typeof(T).Name, page, pageSize);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kayýtlarý getirilirken beklenmeyen bir hata oluþtu. " +
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
                "ID ile kayýt getiriliyor - Varlýk: {EntityType}, ID: {Id}, Takip: {TrackChanges}",
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
                        "Kayýt bulunamadý - Varlýk: {EntityType}, ID: {Id}",
                        typeof(T).Name, id);
                    return null;
                }

                logger.LogDebug(
                    "Kayýt baþarýyla bulundu - Varlýk: {EntityType}, ID: {Id}",
                    typeof(T).Name, id);
                return result;
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex,
                    "Geçersiz sorgu iþlemi - Varlýk: {EntityType}, ID: {Id}, Hata: {Message}",
                    typeof(T).Name, id, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kaydý (ID: {id}) getirilirken geçersiz sorgu hatasý oluþtu. " +
                    "Lütfen include parametrelerini kontrol edin.",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen hata - Varlýk: {EntityType}, ID: {Id}",
                    typeof(T).Name, id);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kaydý (ID: {id}) getirilirken beklenmeyen bir hata oluþtu. " +
                    "Lütfen daha sonra tekrar deneyin.",
                    ex);
            }
        }

        private void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1)
            {
                logger.LogWarning(
                    "Geçersiz sayfa numarasý düzeltildi - Ýstenen: {RequestedPage}, Kullanýlan: 1",
                    page);
                page = 1;
            }

            if (pageSize < 1)
            {
                logger.LogWarning(
                    "Geçersiz sayfa boyutu düzeltildi - Ýstenen: {RequestedSize}, Kullanýlan: {DefaultSize}",
                    pageSize, DefaultPageSize);
                pageSize = DefaultPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning(
                    "Sayfa boyutu sýnýrlandýrýldý - Ýstenen: {RequestedSize}, Kullanýlan: {MaxSize}, Maksimum: {MaxPageSize}",
                    pageSize, MaxPageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }
        }

        private void ValidateId(TKey id)
        {
            if (id == null)
            {
                logger.LogError(
                    "Null ID parametresi - Varlýk: {EntityType}",
                    typeof(T).Name);
                throw new ArgumentNullException(
                    nameof(id),
                    $"{typeof(T).Name} için ID parametresi null olamaz.");
            }

            if (id.Equals(default(TKey)))
            {
                logger.LogError(
                    "Varsayýlan ID deðeri - Varlýk: {EntityType}, ID Tipi: {KeyType}",
                    typeof(T).Name, typeof(TKey).Name);
                throw new ArgumentException(
                    $"{typeof(T).Name} için geçersiz ID deðeri saðlandý. ID varsayýlan deðer olamaz.",
                    nameof(id));
            }
        }
    }
}
