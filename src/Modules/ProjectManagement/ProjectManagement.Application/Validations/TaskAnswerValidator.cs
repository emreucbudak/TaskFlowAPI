using FluentValidation;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Validations
{
    public class TaskAnswerValidator : AbstractValidator<TaskAnswer>
    {
        public TaskAnswerValidator()
        {
            RuleFor(x => x.AnswerText)
                .NotEmpty().WithMessage("Cevap metni boş olamaz.")
                .MaximumLength(1000).WithMessage("Cevap metni 1000 karakterden fazla olamaz.");

            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("Gönderen bilgisi boş olamaz.");
        }
    }
}
