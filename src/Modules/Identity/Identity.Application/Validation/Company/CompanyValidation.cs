using FluentValidation;


namespace Identity.Application.Validation.Company
{
    public class CompanyValidation : AbstractValidator<Identity.Domain.Entities.Company>
    {
        public CompanyValidation()
        {
            RuleFor(c=> c.CompanyName)
                .NotEmpty().WithMessage("Şirket ismi boş olamaz.")
                .NotNull().WithMessage("Şirket ismi null olamaz.")
                .MinimumLength(5).WithMessage("Şirket ismi minimum 5 karakter olmalıdır.")
                .MaximumLength(50).WithMessage("Şirket ismi maximum 50 karakter olmalıdır.");
        }
    }
}
