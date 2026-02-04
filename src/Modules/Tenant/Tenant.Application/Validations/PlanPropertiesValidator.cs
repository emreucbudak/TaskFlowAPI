using FluentValidation;
using Tenant.Domain.Entities;

namespace Tenant.Application.Validations
{
    public class PlanPropertiesValidator : AbstractValidator<PlanProperties>
    {
        public PlanPropertiesValidator()
        {
            RuleFor(x => x.PeopleAddedLimit)
                .GreaterThanOrEqualTo(0).WithMessage("Çalışan ekleme limiti negatif olamaz.");

            RuleFor(x => x.TeamLimit)
                .GreaterThanOrEqualTo(0).WithMessage("Takım ekleme limiti negatif olamaz.");
        }
    }
}
