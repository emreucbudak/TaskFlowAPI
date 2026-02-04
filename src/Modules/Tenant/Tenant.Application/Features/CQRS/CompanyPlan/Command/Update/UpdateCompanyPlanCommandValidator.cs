using FluentValidation;

namespace Tenant.Application.Features.CQRS.CompanyPlan.Command.Update
{
    public class UpdateCompanyPlanCommandValidator : AbstractValidator<UpdateCompanyPlanCommandRequest>
    {
        public UpdateCompanyPlanCommandValidator()
        {
            RuleFor(x => x.CompanyPlanId)
                .NotEmpty().WithMessage("Plan ID'si boş olamaz.");

            RuleFor(x => x.PeopleAddedLimit)
                .GreaterThanOrEqualTo(0).WithMessage("Çalışan ekleme limiti negatif olamaz.");

            RuleFor(x => x.TeamLimit)
                .GreaterThanOrEqualTo(0).WithMessage("Takım ekleme limiti negatif olamaz.");
        }
    }
}
