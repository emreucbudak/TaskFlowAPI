using FluentValidation;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Delete
{
    public class DeleteCompanyPlanCommandValidator : AbstractValidator<DeleteCompanyPlanCommandRequest>
    {
        public DeleteCompanyPlanCommandValidator()
        {
            RuleFor(x => x.CompanyPlanId)
                .NotEmpty().WithMessage("Plan ID'si boş olamaz.");
        }
    }
}
