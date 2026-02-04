using FluentValidation;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Create
{
    public class CreateCompanyPlanCommandValidator : AbstractValidator<CreateCompanyPlanCommandRequest>
    {
        public CreateCompanyPlanCommandValidator()
        {
            RuleFor(x => x.PlanName)
                .NotEmpty().WithMessage("Plan ismi boş olamaz.")
                .MaximumLength(100).WithMessage("Plan ismi en fazla 100 karakter olabilir.");

            RuleFor(x => x.PlanProperties)
                .NotNull().WithMessage("Plan özellikleri boş olamaz.");
        }
    }
}