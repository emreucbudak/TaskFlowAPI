using FluentValidation;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Application.Validations
{
    public class TaskValidator : AbstractValidator<ProjectManagement.Domain.Entities.Task>
    {
        public TaskValidator()
        {
            RuleFor(x => x.TaskName)
                .NotEmpty().WithMessage("Görev adı boş olamaz.")
                .MaximumLength(200).WithMessage("Görev adı 200 karakterden fazla olamaz.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Görev açıklaması boş olamaz.")
                .MaximumLength(2000).WithMessage("Görev açıklaması 2000 karakterden fazla olamaz.");

            RuleFor(x => x.DeadlineTime)
                .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Bitiş tarihi geçmiş bir tarih olamaz.");

            RuleFor(x => x.TaskStatusId)
                .NotEmpty().WithMessage("Görev durumu belirtilmelidir.");
        }
    }
}
