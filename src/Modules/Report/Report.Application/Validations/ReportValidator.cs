using FluentValidation;
using Report.Domain.Entities;

namespace Report.Application.Validations
{
    public class ReportValidator : AbstractValidator<Domain.Entities.Report>
    {
        public ReportValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Rapor başlığı boş olamaz.")
                .MaximumLength(200).WithMessage("Rapor başlığı 200 karakterden fazla olamaz.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Rapor içeriği boş olamaz.");

            RuleFor(x => x.RequesterUserId)
                .NotEmpty().WithMessage("Raporu talep eden kullanıcı bilgisi zorunludur.");
            
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Geçersiz rapor türü.");
        }
    }
}
