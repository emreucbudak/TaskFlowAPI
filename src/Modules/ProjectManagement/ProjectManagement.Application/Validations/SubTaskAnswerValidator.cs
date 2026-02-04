using FluentValidation;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Validations
{
    public class SubTaskAnswerValidator : AbstractValidator<SubTaskAnswer>
    {
        public SubTaskAnswerValidator()
        {
            RuleFor(x => x.AnswerText)
                .NotEmpty().WithMessage("Cevap metni boş olamaz.")
                .MaximumLength(1000).WithMessage("Cevap metni 1000 karakterden fazla olamaz.");

            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("Gönderen bilgisi boş olamaz.");
        }
    }
}
