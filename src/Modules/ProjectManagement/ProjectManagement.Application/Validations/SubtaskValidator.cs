using FluentValidation;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Validations
{
    public class SubtaskValidator : AbstractValidator<Subtask>
    {
        public SubtaskValidator()
        {
            RuleFor(x => x.TaskTitle)
                .NotEmpty().WithMessage("Alt görev başlığı boş olamaz.")
                .MaximumLength(200).WithMessage("Alt görev başlığı 200 karakterden fazla olamaz.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Alt görev açıklaması boş olamaz.")
                .MaximumLength(1000).WithMessage("Alt görev açıklaması 1000 karakterden fazla olamaz.");

            RuleFor(x => x.AssignedUserId)
                .NotEmpty().WithMessage("Atanan kullanıcı bilgisi boş olamaz.");

            RuleFor(x => x.TaskStatusId)
                .NotEmpty().WithMessage("Alt görev durumu belirtilmelidir.");
        }
    }
}
