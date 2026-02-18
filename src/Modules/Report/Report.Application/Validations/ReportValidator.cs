using FluentValidation;

namespace Report.Application.Validations
{
    public class ReportValidator : AbstractValidator<Report.Domain.Entities.Report>
    {
        public ReportValidator()
        {
            RuleFor(r => r.Description)
                .NotEmpty().WithMessage("Açıklama boş olamaz.")
                .MinimumLength(10).WithMessage("Açıklama en az 10 karakter olmalıdır.");

            RuleFor(r => r.ReportTopicId)
                .GreaterThan(0).WithMessage("Geçerli bir rapor konusu seçilmelidir.");

            RuleFor(r => r.ReportingUserId)
                .NotEmpty().WithMessage("Kullanıcı ID'si boş olamaz.");
        }
    }
}
