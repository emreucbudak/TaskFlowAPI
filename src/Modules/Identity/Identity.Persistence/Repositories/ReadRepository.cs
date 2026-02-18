using Identity.Application.Repositories;
using Identity.Persistence.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Common;
using Identity.Domain.Entities;

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
                "Kay�tlar getiriliyor - Varl�k: {EntityType}, Sayfa: {Page}/{TotalPages}, Boyut: {PageSize}",
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
                        "Kay�t say�s� limiti a��yor - Varl�k: {EntityType}, Toplam: {TotalCount}, Limit: {MaxLimit}, �neri: Filtreleme kullan�n",
                        typeof(T).Name, totalCount, MaxTotalRecords);
                }

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                logger.LogInformation(
                    "Kay�tlar ba�ar�yla getirildi - Varl�k: {EntityType}, Getirilen: {ItemCount}/{TotalCount}, Sayfa: {Page}/{TotalPages}",
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
                    "Ge�ersiz sorgu i�lemi - Varl�k: {EntityType}, Sayfa: {Page}, Hata: {Message}",
                    typeof(T).Name, page, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kay�tlar� getirilirken ge�ersiz sorgu hatas� olu�tu. " +
                    "L�tfen include parametrelerini ve filtreleri kontrol edin.",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen hata - Varl�k: {EntityType}, Sayfa: {Page}, Boyut: {PageSize}",
                    typeof(T).Name, page, pageSize);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kay�tlar� getirilirken beklenmeyen bir hata olu�tu. " +
                    "L�tfen daha sonra tekrar deneyin.",
                    ex);
            }
        }

        public async Task<Guid> GetDepartmentLeaderIdAsync(Guid departmentId)
        {
             var leader = await context.Set<DepartmentMember>()
                 .AsNoTracking()
                 .FirstOrDefaultAsync(x => x.DepartmentId == departmentId && x.DepartmentRoleId == 1);
                 
             return leader.UserId;
        }

        public async Task<T> GetByIdAsync(
            bool trackChanges,
            TKey id,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            ValidateId(id);

            logger.LogInformation(
                "ID ile kay�t getiriliyor - Varl�k: {EntityType}, ID: {Id}, Takip: {TrackChanges}",
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
                        "Kay�t bulunamad� - Varl�k: {EntityType}, ID: {Id}",
                        typeof(T).Name, id);
                    return null;
                }

                logger.LogDebug(
                    "Kay�t ba�ar�yla bulundu - Varl�k: {EntityType}, ID: {Id}",
                    typeof(T).Name, id);
                return result;
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex,
                    "Ge�ersiz sorgu i�lemi - Varl�k: {EntityType}, ID: {Id}, Hata: {Message}",
                    typeof(T).Name, id, ex.Message);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kayd� (ID: {id}) getirilirken ge�ersiz sorgu hatas� olu�tu. " +
                    "L�tfen include parametrelerini kontrol edin.",
                    ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Beklenmeyen hata - Varl�k: {EntityType}, ID: {Id}",
                    typeof(T).Name, id);
                throw new InvalidOperationException(
                    $"{typeof(T).Name} kayd� (ID: {id}) getirilirken beklenmeyen bir hata olu�tu. " +
                    "L�tfen daha sonra tekrar deneyin.",
                    ex);
            }
        }

        private void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1)
            {
                logger.LogWarning(
                    "Ge�ersiz sayfa numaras� d�zeltildi - �stenen: {RequestedPage}, Kullan�lan: 1",
                    page);
                page = 1;
            }

            if (pageSize < 1)
            {
                logger.LogWarning(
                    "Ge�ersiz sayfa boyutu d�zeltildi - �stenen: {RequestedSize}, Kullan�lan: {DefaultSize}",
                    pageSize, DefaultPageSize);
                pageSize = DefaultPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning(
                    "Sayfa boyutu s�n�rland�r�ld� - �stenen: {RequestedSize}, Kullan�lan: {MaxSize}, Maksimum: {MaxPageSize}",
                    pageSize, MaxPageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }
        }

        private void ValidateId(TKey id)
        {
            if (id == null)
            {
                logger.LogError(
                    "Null ID parametresi - Varl�k: {EntityType}",
                    typeof(T).Name);
                throw new ArgumentNullException(
                    nameof(id),
                    $"{typeof(T).Name} i�in ID parametresi null olamaz.");
            }

            if (id.Equals(default(TKey)))
            {
                logger.LogError(
                    "Varsay�lan ID de�eri - Varl�k: {EntityType}, ID Tipi: {KeyType}",
                    typeof(T).Name, typeof(TKey).Name);
                throw new ArgumentException(
                    $"{typeof(T).Name} i�in ge�ersiz ID de�eri sa�land�. ID varsay�lan de�er olamaz.",
                    nameof(id));
            }
        }
    }
}
