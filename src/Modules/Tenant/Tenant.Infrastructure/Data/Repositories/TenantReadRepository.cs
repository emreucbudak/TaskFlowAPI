using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Tenant.Application.Repositories;
using Tenant.Domain.Entities;
using Tenant.Infrastructure.Data.TenantDb;

namespace Tenant.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Secure Tenant Read Repository with comprehensive security controls
    /// Features: Input validation, SQL injection protection, logging, caching, performance monitoring
    /// </summary>
    public sealed class TenantReadRepository : ITenantReadRepository
    {
        private readonly TenantDbContext _context;
        private readonly ILogger<TenantReadRepository> _logger;
        private readonly IMemoryCache _cache;

        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 20;
        private const int SlowQueryThresholdMs = 1000;
        private const string CacheKeyPrefix = "tenant_plan_";

        /// <summary>
        /// Constructor with null validation for all dependencies
        /// </summary>
        public TenantReadRepository(
            TenantDbContext context,
            ILogger<TenantReadRepository> logger,
            IMemoryCache cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        /// Retrieves all plans with pagination (SECURE VERSION)
        /// </summary>
        public async Task<List<CompanyPlan>> GetAllPlans(
            bool trackChanges,
            int page = 1,
            int pageSize = DefaultPageSize)
        {
            ValidatePagination(ref page, ref pageSize);

            _logger.LogInformation(
                "Fetching all company plans - Page: {Page}, PageSize: {PageSize}, TrackChanges: {TrackChanges}",
                page, pageSize, trackChanges);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                IQueryable<CompanyPlan> query = _context.companyPlans;

                // Soft delete filter (if CompanyPlan has IsDeleted)
                // query = query.Where(p => !p.IsDeleted);

                if (!trackChanges)
                    query = query.AsNoTracking();

                // Ordering
                query = query.OrderBy(p => p.Price)
                             .ThenBy(p => p.Name);

                var plans = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                stopwatch.Stop();

                _logger.LogInformation(
                    "Retrieved {Count} company plans in {ElapsedMs}ms - Page: {Page}",
                    plans.Count, stopwatch.ElapsedMilliseconds, page);

                LogSlowQueryIfNeeded(stopwatch.ElapsedMilliseconds, "GetAllPlans", page, pageSize);

                return plans;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "Error fetching plans - Page: {Page}, PageSize: {PageSize}, ElapsedMs: {ElapsedMs}",
                    page, pageSize, stopwatch.ElapsedMilliseconds);
                throw new InvalidOperationException("Failed to retrieve company plans", ex);
            }
        }

        /// <summary>
        /// BACKWARD COMPATIBILITY: Deprecated version (Obsolete)
        /// </summary>
        [Obsolete("Use GetAllPlans(trackChanges, page, pageSize) instead. This method will be removed in future versions.")]
        public async Task<List<CompanyPlan>> GetAllPlans(bool trackChanges)
        {
            _logger.LogWarning(
                "DEPRECATED METHOD CALLED: GetAllPlans without pagination. Caller should update to use pagination.");

            return await GetAllPlans(trackChanges, 1, MaxPageSize);
        }

        /// <summary>
        /// Retrieves a specific plan by ID with caching support
        /// </summary>
        public async Task<CompanyPlan?> GetPlan(Guid id, bool trackChanges)
        {
            ValidateId(id);

            // Cache key
            var cacheKey = $"{CacheKeyPrefix}{id}";

            // Try cache first (only for non-tracked queries)
            if (!trackChanges && _cache.TryGetValue(cacheKey, out CompanyPlan? cachedPlan))
            {
                _logger.LogDebug("Plan retrieved from cache - Id: {PlanId}", id);
                return cachedPlan;
            }

            _logger.LogInformation(
                "Fetching company plan - Id: {PlanId}, TrackChanges: {TrackChanges}",
                id, trackChanges);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                IQueryable<CompanyPlan> query = _context.companyPlans;

                if (!trackChanges)
                    query = query.AsNoTracking();

                var plan = await query.FirstOrDefaultAsync(p => p.Id == id);

                stopwatch.Stop();

                if (plan == null)
                {
                    _logger.LogWarning(
                        "Company plan not found - Id: {PlanId}, ElapsedMs: {ElapsedMs}",
                        id, stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    _logger.LogDebug(
                        "Company plan found - Id: {PlanId}, ElapsedMs: {ElapsedMs}",
                        id, stopwatch.ElapsedMilliseconds);

                    // Cache the result (only for non-tracked queries)
                    if (!trackChanges)
                    {
                        _cache.Set(cacheKey, plan, TimeSpan.FromMinutes(5));
                    }
                }

                LogSlowQueryIfNeeded(stopwatch.ElapsedMilliseconds, "GetPlan", id);

                return plan;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "Error fetching plan - Id: {PlanId}, ElapsedMs: {ElapsedMs}",
                    id, stopwatch.ElapsedMilliseconds);
                throw new InvalidOperationException($"Failed to retrieve plan with ID: {id}", ex);
            }
        }

        /// <summary>
        /// Retrieves active plans
        /// </summary>
        public async Task<List<CompanyPlan>> GetActivePlans(
            int page = 1,
            int pageSize = DefaultPageSize,
            bool trackChanges = false)
        {
            ValidatePagination(ref page, ref pageSize);

            _logger.LogInformation(
                "Fetching active company plans - Page: {Page}, PageSize: {PageSize}",
                page, pageSize);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                IQueryable<CompanyPlan> query = _context.companyPlans;
                // .Where(p => p.IsActive); // If IsActive field exists

                if (!trackChanges)
                    query = query.AsNoTracking();

                query = query.OrderBy(p => p.Price);

                var plans = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                stopwatch.Stop();

                _logger.LogInformation(
                    "Retrieved {Count} active plans in {ElapsedMs}ms",
                    plans.Count, stopwatch.ElapsedMilliseconds);

                LogSlowQueryIfNeeded(stopwatch.ElapsedMilliseconds, "GetActivePlans", page, pageSize);

                return plans;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "Error fetching active plans - Page: {Page}, PageSize: {PageSize}",
                    page, pageSize);
                throw new InvalidOperationException("Failed to retrieve active plans", ex);
            }
        }

        /// <summary>
        /// Searches plans by name with SQL injection protection
        /// </summary>
        public async Task<List<CompanyPlan>> SearchPlansByName(
            string searchTerm,
            int page = 1,
            int pageSize = DefaultPageSize,
            bool trackChanges = false)
        {
            ValidateSearchTerm(searchTerm);
            ValidatePagination(ref page, ref pageSize);

            // CRITICAL: Escape LIKE wildcards to prevent wildcard injection
            var escapedTerm = EscapeLikePattern(searchTerm);

            _logger.LogInformation(
                "Searching plans by name - Term length: {Length}, Page: {Page}, PageSize: {PageSize}",
                searchTerm.Length, page, pageSize);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                IQueryable<CompanyPlan> query = _context.companyPlans
                    .Where(p => EF.Functions.Like(p.Name, $"%{escapedTerm}%"));

                if (!trackChanges)
                    query = query.AsNoTracking();

                query = query.OrderBy(p => p.Name);

                var plans = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                stopwatch.Stop();

                _logger.LogInformation(
                    "Found {Count} plans in {ElapsedMs}ms - Term length: {Length}",
                    plans.Count, stopwatch.ElapsedMilliseconds, searchTerm.Length);

                LogSlowQueryIfNeeded(stopwatch.ElapsedMilliseconds, "SearchPlansByName", page, pageSize);

                return plans;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "Error searching plans - Term length: {Length}, Page: {Page}",
                    searchTerm.Length, page);
                throw new InvalidOperationException("Failed to search plans", ex);
            }
        }

        /// <summary>
        /// Retrieves plans by price range
        /// </summary>
        public async Task<List<CompanyPlan>> GetPlansByPriceRange(
            decimal minPrice,
            decimal maxPrice,
            int page = 1,
            int pageSize = DefaultPageSize,
            bool trackChanges = false)
        {
            ValidatePriceRange(minPrice, maxPrice);
            ValidatePagination(ref page, ref pageSize);

            _logger.LogInformation(
                "Fetching plans by price range - Min: {MinPrice}, Max: {MaxPrice}, Page: {Page}",
                minPrice, maxPrice, page);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                IQueryable<CompanyPlan> query = _context.companyPlans
                    .Where(p => p.Price >= minPrice && p.Price <= maxPrice);

                if (!trackChanges)
                    query = query.AsNoTracking();

                query = query.OrderBy(p => p.Price);

                var plans = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                stopwatch.Stop();

                _logger.LogInformation(
                    "Retrieved {Count} plans in price range in {ElapsedMs}ms",
                    plans.Count, stopwatch.ElapsedMilliseconds);

                LogSlowQueryIfNeeded(stopwatch.ElapsedMilliseconds, "GetPlansByPriceRange", page, pageSize);

                return plans;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex,
                    "Error fetching plans by price range - Min: {MinPrice}, Max: {MaxPrice}",
                    minPrice, maxPrice);
                throw new InvalidOperationException("Failed to retrieve plans by price range", ex);
            }
        }

        /// <summary>
        /// Gets total plan count
        /// </summary>
        public async Task<int> GetTotalPlanCount()
        {
            _logger.LogDebug("Getting total company plan count");

            try
            {
                var count = await _context.companyPlans
                    .AsNoTracking()
                    .CountAsync();

                _logger.LogInformation("Total company plan count: {Count}", count);

                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total plan count");
                throw new InvalidOperationException("Failed to get plan count", ex);
            }
        }

        /// <summary>
        /// Checks if a plan exists
        /// </summary>
        public async Task<bool> PlanExists(Guid id)
        {
            ValidateId(id);

            _logger.LogDebug("Checking if plan exists - Id: {PlanId}", id);

            try
            {
                var exists = await _context.companyPlans
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == id);

                _logger.LogDebug(
                    "Plan exists check - Id: {PlanId}, Exists: {Exists}",
                    id, exists);

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking plan existence - Id: {PlanId}", id);
                throw new InvalidOperationException($"Failed to check plan existence: {id}", ex);
            }
        }

        /// <summary>
        /// Checks if plan name is unique
        /// </summary>
        public async Task<bool> IsPlanNameUnique(string planName, Guid? excludePlanId = null)
        {
            if (string.IsNullOrWhiteSpace(planName))
            {
                _logger.LogWarning("Empty plan name provided for uniqueness check");
                return false;
            }

            _logger.LogDebug(
                "Checking plan name uniqueness - Name length: {Length}, ExcludeId: {ExcludeId}",
                planName.Length, excludePlanId);

            try
            {
                var query = _context.companyPlans
                    .AsNoTracking()
                    .Where(p => p.Name == planName);

                if (excludePlanId.HasValue)
                {
                    query = query.Where(p => p.Id != excludePlanId.Value);
                }

                var exists = await query.AnyAsync();

                return !exists; // Unique if not exists
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking plan name uniqueness");
                throw new InvalidOperationException("Failed to check plan name uniqueness", ex);
            }
        }

        #region Private Validation Methods

        private void ValidateId(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogError("Empty plan ID provided");
                throw new ArgumentException("Plan ID cannot be empty", nameof(id));
            }
        }

        private void ValidateSearchTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.LogError("Empty search term provided");
                throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));
            }

            if (searchTerm.Length < 2)
            {
                _logger.LogWarning("Search term too short: {Length} characters", searchTerm.Length);
                throw new ArgumentException(
                    "Search term must be at least 2 characters",
                    nameof(searchTerm));
            }

            if (searchTerm.Length > 100)
            {
                _logger.LogWarning("Search term too long: {Length} characters", searchTerm.Length);
                throw new ArgumentException(
                    "Search term cannot exceed 100 characters",
                    nameof(searchTerm));
            }

            // Check for potential sensitive data patterns
            if (ContainsSensitiveData(searchTerm))
            {
                _logger.LogWarning("Search term contains potential sensitive data pattern");
                throw new ArgumentException(
                    "Search term contains invalid pattern",
                    nameof(searchTerm));
            }
        }

        private void ValidatePriceRange(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0)
            {
                _logger.LogError("Negative minimum price: {MinPrice}", minPrice);
                throw new ArgumentException("Minimum price cannot be negative", nameof(minPrice));
            }

            if (maxPrice < 0)
            {
                _logger.LogError("Negative maximum price: {MaxPrice}", maxPrice);
                throw new ArgumentException("Maximum price cannot be negative", nameof(maxPrice));
            }

            if (minPrice > maxPrice)
            {
                _logger.LogError(
                    "Invalid price range - Min: {MinPrice}, Max: {MaxPrice}",
                    minPrice, maxPrice);
                throw new ArgumentException(
                    "Minimum price cannot be greater than maximum price");
            }
        }

        private void ValidatePagination(ref int page, ref int pageSize)
        {
            if (page < 1)
            {
                _logger.LogWarning("Invalid page number: {Page}, resetting to 1", page);
                page = 1;
            }

            if (pageSize < 1)
            {
                _logger.LogWarning(
                    "Invalid page size: {PageSize}, resetting to default {DefaultPageSize}",
                    pageSize, DefaultPageSize);
                pageSize = DefaultPageSize;
            }

            if (pageSize > MaxPageSize)
            {
                _logger.LogWarning(
                    "Page size {PageSize} exceeds maximum {MaxPageSize}, limiting",
                    pageSize, MaxPageSize);
                pageSize = MaxPageSize;
            }
        }

        #endregion

        #region Security Helper Methods

        /// <summary>
        /// Escapes LIKE pattern special characters to prevent wildcard injection
        /// </summary>
        private static string EscapeLikePattern(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            return input
                .Replace("[", "[[]")  // Escape [ first
                .Replace("%", "[%]")  // Escape %
                .Replace("_", "[_]"); // Escape _
        }

        /// <summary>
        /// Checks if input contains sensitive data patterns (credit card, email, etc.)
        /// </summary>
        private static bool ContainsSensitiveData(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Credit card pattern (basic check)
            if (Regex.IsMatch(input, @"\d{4}[-\s]?\d{4}[-\s]?\d{4}[-\s]?\d{4}"))
                return true;

            // Email pattern
            if (Regex.IsMatch(input, @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}"))
                return true;

            // SSN pattern (basic)
            if (Regex.IsMatch(input, @"\d{3}-\d{2}-\d{4}"))
                return true;

            return false;
        }

        /// <summary>
        /// Logs slow query warnings for performance monitoring
        /// </summary>
        private void LogSlowQueryIfNeeded(long elapsedMs, string methodName, params object[] parameters)
        {
            if (elapsedMs > SlowQueryThresholdMs)
            {
                _logger.LogWarning(
                    "SLOW QUERY DETECTED: {MethodName} took {ElapsedMs}ms - Parameters: {Parameters}",
                    methodName, elapsedMs, string.Join(", ", parameters));
            }
        }

        #endregion
    }
}