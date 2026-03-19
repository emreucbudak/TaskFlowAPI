using FluentValidation;
using Report.Domain.Entities;

namespace Report.Application.Validations
{
    public class ReportStatusValidator : AbstractValidator<ReportStatus>
    {
        public ReportStatusValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Durum adı boş olamaz.")
                .MaximumLength(100).WithMessage("Durum adı en fazla 100 karakter olabilir.");
        }
    }
}
