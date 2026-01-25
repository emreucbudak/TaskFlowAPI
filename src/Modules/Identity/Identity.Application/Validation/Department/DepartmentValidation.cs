using FluentValidation;

namespace Identity.Application.Validation.Department
{
    public class DepartmentValidation : AbstractValidator<Domain.Entities.Department>
    {
        public DepartmentValidation()
        {
            RuleFor(d=> d.Name)
                .NotEmpty().WithMessage("Departman adı boş olamaz.")
                .MaximumLength(10).WithMessage("Departman adı maximum 10 karakter olmalı.")
                .MinimumLength(2).WithMessage("Departman adı minimum 2 karakter olmalıdır.")
                .NotNull().WithMessage("Departman adı null olamaz.");

        }
    }
}
