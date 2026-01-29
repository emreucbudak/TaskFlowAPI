using Identity.Application.Repositories;
using Identity.Infrastructure.Data.IdentityDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskFlow.BuildingBlocks.Common;

namespace Identity.Infrastructure.Repository
{
    // Soft delete için interface
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        Guid? DeletedBy { get; set; }
    }

    // User ownership için interface
    public interface IUserOwnedEntity
    {
        Guid UserId { get; set; }
    }

    // Concurrency için interface
    public interface IHasConcurrencyToken
    {
        byte[] RowVersion { get; set; }
    }

    // Current user service (dependency olarak eklenmeli)
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        bool IsAuthenticated { get; }
    }

    /// <summary>
    /// Güvenli Write Repository - Authorization, Validation, Logging ile
    /// </summary>
    public class WriteRepository<T>(
        IdentityManagementDbContext context,
        ILogger<WriteRepository<T>> logger,
        ICurrentUserService currentUserService) : IWriteRepository<T>
        where T : class
    {
        private DbSet<T> db => context.Set<T>();

        /// <summary>
        /// Yeni entity ekler (Validation + Authorization + Logging)
        /// NOT: async kaldırıldı çünkü EF Core Add() sync
        /// </summary>
        public void Add(T entity)
        {
            // Input validation
            ValidateEntity(entity, nameof(Add));

            // Authorization check
            ValidateAddPermission(entity);

            // Creatable entity ise created bilgilerini set et
            if (entity is ICreatableEntity creatable)
            {
                creatable.CreatedAt = DateTime.UtcNow;
                creatable.CreatedBy = currentUserService.UserId;
            }

            logger.LogInformation(
                "Adding entity - Type: {Type}, User: {UserId}",
                typeof(T).Name,
                currentUserService.UserId);

            db.Add(entity);

            logger.LogDebug(
                "Entity added to context - Type: {Type}",
                typeof(T).Name);
        }

        /// <summary>
        /// Entity'yi günceller (Validation + Authorization + Concurrency + Logging)
        /// </summary>
        public void Update(T entity)
        {
            // Input validation
            ValidateEntity(entity, nameof(Update));

            // Authorization check
            ValidateUpdatePermission(entity);

            // Updatable entity ise updated bilgilerini set et
            if (entity is IUpdatableEntity updatable)
            {
                updatable.UpdatedAt = DateTime.UtcNow;
                updatable.UpdatedBy = currentUserService.UserId;
            }

            // Concurrency check
            if (entity is IHasConcurrencyToken concurrency)
            {
                var entry = context.Entry(entity);
                entry.Property(nameof(IHasConcurrencyToken.RowVersion))
                    .OriginalValue = concurrency.RowVersion;

                logger.LogDebug(
                    "Concurrency token set for entity - Type: {Type}",
                    typeof(T).Name);
            }

            logger.LogInformation(
                "Updating entity - Type: {Type}, User: {UserId}",
                typeof(T).Name,
                currentUserService.UserId);

            db.Update(entity);
        }

        /// <summary>
        /// Entity'yi siler (Soft Delete Destekli + Authorization + Logging)
        /// </summary>
        public void Delete(T entity)
        {
            // Input validation
            ValidateEntity(entity, nameof(Delete));

            // Authorization check
            ValidateDeletePermission(entity);

            // Soft delete kontrolü
            if (entity is ISoftDelete softDelete)
            {
                // Soft delete
                softDelete.IsDeleted = true;
                softDelete.DeletedAt = DateTime.UtcNow;
                softDelete.DeletedBy = currentUserService.UserId;

                db.Update(entity);

                logger.LogInformation(
                    "Soft deleted entity - Type: {Type}, User: {UserId}",
                    typeof(T).Name,
                    currentUserService.UserId);
            }
            else
            {
                // Hard delete (dikkatli!)
                db.Remove(entity);

                logger.LogWarning(
                    "Hard deleted entity - Type: {Type}, User: {UserId}",
                    typeof(T).Name,
                    currentUserService.UserId);
            }
        }

        /// <summary>
        /// Hard delete (Soft delete olsa bile kalıcı silme)
        /// Dikkatli kullanılmalı! Sadece admin veya system operations için
        /// </summary>
        public void PermanentDelete(T entity)
        {
            ValidateEntity(entity, nameof(PermanentDelete));
            ValidateDeletePermission(entity);

            logger.LogCritical(
                "PERMANENT DELETE - Type: {Type}, User: {UserId}",
                typeof(T).Name,
                currentUserService.UserId);

            db.Remove(entity);
        }

        #region Validation Methods

        private void ValidateEntity(T entity, string operation)
        {
            if (entity == null)
            {
                logger.LogError(
                    "Null entity provided to {Operation}",
                    operation);
                throw new ArgumentNullException(
                    nameof(entity),
                    $"Entity cannot be null for {operation} operation");
            }

            // Entity base type ise ID kontrolü
            if (entity is BaseEntity<Guid> baseEntity)
            {
                if (baseEntity.Id == Guid.Empty && operation != nameof(Add))
                {
                    logger.LogError(
                        "Empty ID for {Operation} operation - Type: {Type}",
                        operation, typeof(T).Name);
                    throw new ArgumentException(
                        "Entity ID cannot be empty for Update/Delete",
                        nameof(entity));
                }
            }
        }

        private void ValidateAddPermission(T entity)
        {
            // User owned entity ise, userId kontrolü yap
            if (entity is IUserOwnedEntity userOwned)
            {
                // Eğer userId set edilmemişse, current user'ı set et
                if (userOwned.UserId == Guid.Empty)
                {
                    userOwned.UserId = currentUserService.UserId;
                    logger.LogDebug(
                        "UserId set to current user - Type: {Type}, UserId: {UserId}",
                        typeof(T).Name, currentUserService.UserId);
                }
                // Eğer farklı bir userId set edilmişse, authorization check
                else if (userOwned.UserId != currentUserService.UserId)
                {
                    logger.LogWarning(
                        "Unauthorized add attempt - Type: {Type}, RequestedUserId: {RequestedId}, CurrentUserId: {CurrentId}",
                        typeof(T).Name, userOwned.UserId, currentUserService.UserId);

                    throw new UnauthorizedAccessException(
                        "You cannot add entities for other users");
                }
            }
        }

        private void ValidateUpdatePermission(T entity)
        {
            // User owned entity ise, sadece kendi entity'sini güncelleyebilir
            if (entity is IUserOwnedEntity userOwned)
            {
                if (userOwned.UserId != currentUserService.UserId)
                {
                    logger.LogWarning(
                        "Unauthorized update attempt - Type: {Type}, EntityUserId: {EntityUserId}, CurrentUserId: {CurrentUserId}",
                        typeof(T).Name, userOwned.UserId, currentUserService.UserId);

                    throw new UnauthorizedAccessException(
                        "You cannot update entities that don't belong to you");
                }
            }
        }

        private void ValidateDeletePermission(T entity)
        {
            // User owned entity ise, sadece kendi entity'sini silebilir
            if (entity is IUserOwnedEntity userOwned)
            {
                if (userOwned.UserId != currentUserService.UserId)
                {
                    logger.LogWarning(
                        "Unauthorized delete attempt - Type: {Type}, EntityUserId: {EntityUserId}, CurrentUserId: {CurrentUserId}",
                        typeof(T).Name, userOwned.UserId, currentUserService.UserId);

                    throw new UnauthorizedAccessException(
                        "You cannot delete entities that don't belong to you");
                }
            }
        }

        #endregion
    }

    #region Helper Interfaces (BaseEntity'den extend edilebilir)

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

    #endregion
}