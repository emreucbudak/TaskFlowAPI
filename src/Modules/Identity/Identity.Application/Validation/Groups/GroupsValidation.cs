using FluentValidation;

namespace Identity.Application.Validation.Groups
{
    public class GroupsValidation : AbstractValidator<Identity.Domain.Entities.Groups>
    {
        public GroupsValidation()
        {
            RuleFor(g=> g.Name)
                .NotEmpty().WithMessage("Grup ismi boş olamaz")
                .MaximumLength(100).WithMessage("Grup ismi maximum 100 karakter olmalıdır")
                .NotNull().WithMessage("Grup ismi null olamaz")
                .MinimumLength(3).WithMessage("Grup ismi minimum 3 karakter olmalıdır");

        }
    }
}
