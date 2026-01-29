using Identity.Application.Repositories;
using Identity.Infrastructure.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Infrastructure.Repository
{
    // Pagination için result model
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

    // Soft delete için interface
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }

    // User ownership için interface
    public interface IUserOwnedEntity
    {
        Guid UserId { get; set; }
    }

    // Multi-tenant için interface
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

        private DbSet<T> db => context.Set<T>();

        /// <summary>
        /// Tüm kayıtları sayfalı olarak getirir (GÜVENLİ)
        /// </summary>
        public async Task<PagedResult<T>> GetAllAsync(
            int page = 1,
            int pageSize = DefaultPageSize,
            bool trackChanges = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            // Pagination validation
            ValidatePagination(ref page, ref pageSize);

            logger.LogInformation(
                "GetAllAsync called - Type: {Type}, Page: {Page}, PageSize: {PageSize}",
                typeof(T).Name, page, pageSize);

            IQueryable<T> query = db.AsQueryable();

            // Soft delete filtresi
            query = ApplySoftDeleteFilter(query);

            if (!trackChanges)
                query = query.AsNoTracking();

            if (inc is not null)
                query = SafeInclude(query, inc);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            logger.LogInformation(
                "GetAllAsync completed - Type: {Type}, Retrieved: {Count}/{Total}",
                typeof(T).Name, items.Count, totalCount);

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// ID'ye göre tek kayıt getirir (GÜVENLİ)
        /// </summary>
        public async Task<T?> GetByIdAsync(
            bool trackChanges,
            TKey id,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            // Input validation
            ValidateId(id);

            logger.LogInformation(
                "GetByIdAsync called - Type: {Type}, ID: {Id}, TrackChanges: {TrackChanges}",
                typeof(T).Name, id, trackChanges);

            IQueryable<T> query = db.AsQueryable();

            // Soft delete filtresi
            query = ApplySoftDeleteFilter(query);

            if (!trackChanges)
                query = query.AsNoTracking();

            if (inc is not null)
                query = SafeInclude(query, inc);

            var result = await query.FirstOrDefaultAsync(x => x.Id!.Equals(id));

            if (result == null)
            {
                logger.LogWarning(
                    "Entity not found - Type: {Type}, ID: {Id}",
                    typeof(T).Name, id);
            }
            else
            {
                logger.LogDebug(
                    "Entity found - Type: {Type}, ID: {Id}",
                    typeof(T).Name, id);
            }

            return result;
        }

        /// <summary>
        /// Authorization kontrollü GetById (Kullanıcı bazlı)
        /// </summary>
        public async Task<T?> GetByIdWithAuthorizationAsync(
            TKey id,
            Guid currentUserId,
            bool trackChanges = false,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? inc = null)
        {
            ValidateId(id);

            logger.LogInformation(
                "GetByIdWithAuthorization - Type: {Type}, ID: {Id}, User: {UserId}",
                typeof(T).Name, id, currentUserId);

            IQueryable<T> query = db.AsQueryable();


            query = ApplySoftDeleteFilter(query);


            query = ApplyAuthorizationFilter(query, currentUserId);

            if (!trackChanges)
                query = query.AsNoTracking();

            if (inc is not null)
                query = SafeInclude(query, inc);

            var result = await query.FirstOrDefaultAsync(x => x.Id!.Equals(id));

            if (result == null)
            {
                logger.LogWarning(
                    "Unauthorized or not found - Type: {Type}, ID: {Id}, User: {UserId}",
                    typeof(T).Name, id, currentUserId);
            }

            return result;
        }


        private void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1)
            {
                logger.LogWarning("Invalid page number: {Page}, resetting to 1", page);
                page = 1;
            }

            if (pageSize < 1)
            {
                logger.LogWarning("Invalid page size: {PageSize}, resetting to default", pageSize);
                pageSize = DefaultPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                logger.LogWarning(
                    "Page size {PageSize} exceeds maximum {MaxPageSize}, limiting",
                    pageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }
        }

        private void ValidateId(TKey id)
        {
            if (id == null)
            {
                logger.LogError("Null ID provided to GetByIdAsync");
                throw new ArgumentNullException(nameof(id), "ID cannot be null");
            }

            if (id.Equals(default(TKey)))
            {
                logger.LogError("Default/Invalid ID provided: {Id}", id);
                throw new ArgumentException("Invalid ID value", nameof(id));
            }
        }

        private IQueryable<T> ApplySoftDeleteFilter(IQueryable<T> query)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {
                logger.LogDebug("Applying soft delete filter to {Type}", typeof(T).Name);

           
                query = query.Where(x => !((ISoftDelete)(object)x).IsDeleted);
            }
            return query;
        }

        private IQueryable<T> ApplyAuthorizationFilter(IQueryable<T> query, Guid userId)
        {
            if (typeof(IUserOwnedEntity).IsAssignableFrom(typeof(T)))
            {
                logger.LogDebug(
                    "Applying user authorization filter - Type: {Type}, User: {UserId}",
                    typeof(T).Name, userId);

 
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

      
                logger.LogDebug("Include applied to query for type {Type}", typeof(T).Name);

                return includedQuery;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error applying include to query for type {Type}",
                    typeof(T).Name);

                throw new InvalidOperationException(
                    "Invalid include operation. Please check your navigation properties.",
                    ex);
            }
        }

    }
}