using FluentValidation;
using Stats.Domain.Entities;

namespace Stats.Application.Validations
{
    public class WorkerStatsValidator : AbstractValidator<WorkerStats>
    {
        public WorkerStatsValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanıcı ID'si boş olamaz.");

            RuleFor(x => x.Period)
                .NotEmpty().WithMessage("Dönem bilgisi boş olamaz.");

            RuleFor(x => x.TotalTasksAssigned)
                .GreaterThanOrEqualTo(0).WithMessage("Atanan toplam görev sayısı negatif olamaz.");

            RuleFor(x => x.TotalTasksCompleted)
                .GreaterThanOrEqualTo(0).WithMessage("Tamamlanan toplam görev sayısı negatif olamaz.");

            RuleFor(x => x.TasksCompletedBeforeDeadline)
                .GreaterThanOrEqualTo(0).WithMessage("Süresinden önce tamamlanan görev sayısı negatif olamaz.")
                .LessThanOrEqualTo(x => x.TotalTasksCompleted).WithMessage("Süresinden önce tamamlanan görev sayısı, toplam tamamlanan görev sayısından fazla olamaz.");

            RuleFor(x => x.OverdueIncompleteTasksCount)
                .GreaterThanOrEqualTo(0).WithMessage("Süresi geçen tamamlanmamış görev sayısı negatif olamaz.");
        }
    }
}
