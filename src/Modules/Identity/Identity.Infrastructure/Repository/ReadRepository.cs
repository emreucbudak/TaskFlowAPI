using Identity.Application.Repositories;
using Identity.Infrastructure.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Infrastructure.Repository
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }

    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }

    public interface IUserOwnedEntity
    {
        Guid UserId { get; set; }
    }

    public interface IMultiTenant
    {
        Guid TenantId { get; set; }
    }

    public class ReadRepository<T, TKey>(
        IdentityManagementDbContext context,
        ILogger<ReadRepository<T, TKey>> logger) : IReadRepository<T, TKey>
        where T : BaseEntity<TKey>
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;
        private const int MaxIncludeDepth = 3;
        private const int MaxTotalRecords = 10000;

        private DbSet<T> db => context.Set<T>();

        public async Task<PagedResult<T>> GetAllAsync(
            int page = 1,
            int pageSize = DefaultPageSize,
            bool trackChanges = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "Tüm kayıtlar getiriliyor - Tür: {Type}, Sayfa: {Page}, Sayfa Boyutu: {PageSize}",
                typeof(T).Name, page, pageSize);

            try
            {
                IQueryable<T> query = db.AsQueryable();

                query = ApplySoftDeleteFilter(query);

                if (!trackChanges)
                    query = query.AsNoTracking();

                if (inc is not null)
                    query = SafeInclude(query, inc);

                var totalCount = await query.CountAsync();

                if (totalCount > MaxTotalRecords)
                {
                    logger.LogWarning(
                        "Toplam kayıt sayısı limiti aşıyor - Tür: {Type}, Toplam: {Total}, Limit: {Limit}",
                        typeof(T).Name, totalCount, MaxTotalRecords);
                }

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                logger.LogInformation(
                    "Kayıtlar başarıyla getirildi - Tür: {Type}, Getirilen: {Count}/{Total}",
                    typeof(T).Name, items.Count, totalCount);

                return new PagedResult<T>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kayıtlar getirilirken hata oluştu - Tür: {Type}", typeof(T).Name);
                throw;
            }
        }

        public async Task<T?> GetByIdAsync(
            bool trackChanges,
            TKey id,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            ValidateId(id);

            logger.LogInformation(
                "ID ile kayıt getiriliyor - Tür: {Type}, Değişiklik Takibi: {TrackChanges}",
                typeof(T).Name, trackChanges);

            try
            {
                IQueryable<T> query = db.AsQueryable();

                query = ApplySoftDeleteFilter(query);

                if (!trackChanges)
                    query = query.AsNoTracking();

                if (inc is not null)
                    query = SafeInclude(query, inc);

                var result = await query.FirstOrDefaultAsync(x => x.Id!.Equals(id));

                if (result == null)
                {
                    logger.LogWarning("Kayıt bulunamadı - Tür: {Type}", typeof(T).Name);
                }
                else
                {
                    logger.LogDebug("Kayıt bulundu - Tür: {Type}", typeof(T).Name);
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kayıt getirilirken hata oluştu - Tür: {Type}", typeof(T).Name);
                throw;
            }
        }

        public async Task<T?> GetByIdWithAuthorizationAsync(
            TKey id,
            Guid currentUserId,
            bool trackChanges = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            ValidateId(id);

            if (currentUserId == Guid.Empty)
            {
                logger.LogError("Geçersiz kullanıcı ID'si sağlandı");
                throw new ArgumentException("Kullanıcı ID'si geçersiz", nameof(currentUserId));
            }

            logger.LogInformation(
                "Yetkilendirme ile kayıt getiriliyor - Tür: {Type}",
                typeof(T).Name);

            try
            {
                IQueryable<T> query = db.AsQueryable();

                query = ApplySoftDeleteFilter(query);
                query = ApplyAuthorizationFilter(query, currentUserId);

                if (!trackChanges)
                    query = query.AsNoTracking();

                if (inc is not null)
                    query = SafeInclude(query, inc);

                var result = await query.FirstOrDefaultAsync(x => x.Id!.Equals(id));

                if (result is null)
                {
                    logger.LogWarning(
                        "Yetkisiz erişim veya kayıt bulunamadı - Tür: {Type}",
                        typeof(T).Name);
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Yetkilendirmeli kayıt getirme işleminde hata - Tür: {Type}",
                    typeof(T).Name);
                throw;
            }
        }

        private void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1)
            {
                logger.LogWarning("Geçersiz sayfa numarası: {Page}, 1'e ayarlanıyor", page);
                page = 1;
            }

            if (pageSize < 1)
            {
                logger.LogWarning("Geçersiz sayfa boyutu: {PageSize}, varsayılana ayarlanıyor", pageSize);
                pageSize = DefaultPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning(
                    "Sayfa boyutu {PageSize} maksimum değeri {MaxPageSize} aşıyor, sınırlanıyor",
                    pageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }
        }

        private void ValidateId(TKey id)
        {
            if (id == null)
            {
                logger.LogError("Null ID değeri sağlandı");
                throw new ArgumentNullException(nameof(id), "ID null olamaz");
            }

            if (id.Equals(default(TKey)))
            {
                logger.LogError("Geçersiz ID değeri sağlandı");
                throw new ArgumentException("Geçersiz ID değeri", nameof(id));
            }
        }

        private IQueryable<T> ApplySoftDeleteFilter(IQueryable<T> query)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {
                logger.LogDebug("Soft delete filtresi uygulanıyor - Tür: {Type}", typeof(T).Name);
                query = query.Where(x => !((ISoftDelete)(object)x).IsDeleted);
            }
            return query;
        }

        private IQueryable<T> ApplyAuthorizationFilter(IQueryable<T> query, Guid userId)
        {
            if (typeof(IUserOwnedEntity).IsAssignableFrom(typeof(T)))
            {
                logger.LogDebug(
                    "Kullanıcı yetkilendirme filtresi uygulanıyor - Tür: {Type}",
                    typeof(T).Name);
                query = query.Where(x => ((IUserOwnedEntity)(object)x).UserId == userId);
            }
            return query;
        }

        private IQueryable<T> SafeInclude(
            IQueryable<T> query,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> inc)
        {
            try
            {
                var includedQuery = inc(query);
                logger.LogDebug("Include işlemi uygulandı - Tür: {Type}", typeof(T).Name);
                return includedQuery;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Include işlemi uygulanırken hata oluştu - Tür: {Type}",
                    typeof(T).Name);

                throw new InvalidOperationException(
                    "Geçersiz include işlemi. Lütfen navigation property'lerinizi kontrol edin.",
                    ex);
            }
        }
    }
}