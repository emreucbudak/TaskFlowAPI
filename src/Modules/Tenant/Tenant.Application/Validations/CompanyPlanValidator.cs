using FluentValidation;
using Tenant.Domain.Entities;

namespace Tenant.Application.Validations
{
    public class CompanyPlanValidator : AbstractValidator<CompanyPlan>
    {
        public CompanyPlanValidator()
        {
            RuleFor(x => x.PlanName)
                .NotEmpty().WithMessage("Plan ismi boş olamaz.")
                .MaximumLength(100).WithMessage("Plan ismi en fazla 100 karakter olabilir.");

            RuleFor(x => x.PlanPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Plan fiyatı negatif olamaz.");

            RuleFor(x => x.PlanProperties)
                .NotNull().WithMessage("Plan özellikleri boş olamaz.");
        }
    }
}
