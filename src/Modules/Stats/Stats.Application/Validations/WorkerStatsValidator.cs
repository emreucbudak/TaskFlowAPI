using FluentValidation;
using Stats.Domain.Entities;

namespace Stats.Application.Validations
{
    public class WorkerStatsValidator : AbstractValidator<WorkerStats>
    {
        public WorkerStatsValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanici ID'si bos olamaz.");

            RuleFor(x => x.Period)
                .NotEmpty().WithMessage("Donem bilgisi bos olamaz.");

            RuleFor(x => x.TotalTasksAssigned)
                .GreaterThanOrEqualTo(0).WithMessage("Atanan toplam gorev sayisi negatif olamaz.");

            RuleFor(x => x.TotalTasksCompleted)
                .GreaterThanOrEqualTo(0).WithMessage("Tamamlanan toplam gorev sayisi negatif olamaz.");

            RuleFor(x => x.TasksCompletedBeforeDeadline)
                .GreaterThanOrEqualTo(0).WithMessage("Suresinden once tamamlanan gorev sayisi negatif olamaz.")
                .LessThanOrEqualTo(x => x.TotalTasksCompleted).WithMessage("Suresinden once tamamlanan gorev sayisi, toplam tamamlanan gorev sayisindan fazla olamaz.");

            RuleFor(x => x.OverdueIncompleteTasksCount)
                .GreaterThanOrEqualTo(0).WithMessage("Suresi gecen tamamlanmamis gorev sayisi negatif olamaz.");

            RuleFor(x => x.TotalPoints)
                .GreaterThanOrEqualTo(0).WithMessage("Toplam puan negatif olamaz.");
        }
    }
}
