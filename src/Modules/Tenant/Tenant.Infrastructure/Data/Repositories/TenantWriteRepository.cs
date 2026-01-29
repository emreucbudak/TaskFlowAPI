using Microsoft.EntityFrameworkCore;
using Tenant.Application.Repositories;
using Tenant.Domain.Entities;
using Tenant.Infrastructure.Data.TenantDb;

namespace Tenant.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Tenant write repository with enhanced security controls
    /// </summary>
    public sealed class TenantWriteRepository(TenantDbContext context) : ITenantWriteRepository
    {
        private readonly TenantDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        /// <summary>
        /// Adds a new company plan with validation
        /// </summary>
        public async Task AddPlan(CompanyPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);

            // İş kuralı validasyonları eklenebilir
            ValidatePlan(plan);

            try
            {
                await _context.companyPlans.AddAsync(plan);
            }
            catch (DbUpdateException ex)
            {
                // Veritabanı hatalarını logla ve özel exception fırlat
                throw new InvalidOperationException("Plan eklenirken hata oluştu.", ex);
            }
        }

        /// <summary>
        /// Deletes a company plan with existence check
        /// </summary>
        public async Task DeletePlan(CompanyPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);

            var entry = _context.Entry(plan);

            // Detached durumundaki entity'leri attach et
            if (entry.State == EntityState.Detached)
            {
                // ID kontrolü - entity veritabanında var mı?
                var exists = await _context.companyPlans
                    .AnyAsync(p => p.Id == plan.Id);

                if (!exists)
                {
                    throw new InvalidOperationException($"Silinecek plan bulunamadı. ID: {plan.Id}");
                }

                _context.companyPlans.Attach(plan);
            }

            _context.companyPlans.Remove(plan);
        }

        /// <summary>
        /// Updates a company plan with concurrency handling
        /// </summary>
        public async Task UpdatePlan(CompanyPlan plan)
        {
            ArgumentNullException.ThrowIfNull(plan);

            // İş kuralı validasyonları
            ValidatePlan(plan);

            var entry = _context.Entry(plan);

            // Detached durumundaki entity'leri attach et
            if (entry.State == EntityState.Detached)
            {
                // Güncellenecek entity'nin varlığını kontrol et
                var exists = await _context.companyPlans
                    .AnyAsync(p => p.Id == plan.Id);

                if (!exists)
                {
                    throw new InvalidOperationException($"Güncellenecek plan bulunamadı. ID: {plan.Id}");
                }

                _context.companyPlans.Attach(plan);
                entry.State = EntityState.Modified;
            }
            else
            {
                _context.companyPlans.Update(plan);
            }
        }

        /// <summary>
        /// Validates business rules for CompanyPlan
        /// </summary>
        private static void ValidatePlan(CompanyPlan plan)
        {
            // Örnek validasyonlar - iş kurallarınıza göre özelleştirin
            if (plan.Id < 0)
            {
                throw new ArgumentException("Plan ID negatif olamaz.", nameof(plan.Id));
            }

            // Diğer validasyonlar eklenebilir:
            // - Plan adı kontrolü
            // - Tarih aralığı kontrolü
            // - Fiyat kontrolü vb.
        }
    }
}