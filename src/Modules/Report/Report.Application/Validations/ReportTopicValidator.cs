using FluentValidation;
using Report.Domain.Entities;

namespace Report.Application.Validations
{
    public class ReportTopicValidator : AbstractValidator<ReportTopic>
    {
        public ReportTopicValidator()
        {
            RuleFor(x => x.TopicName)
                .NotEmpty().WithMessage("Konu adı boş olamaz.")
                .MaximumLength(200).WithMessage("Konu adı en fazla 200 karakter olabilir.");
        }
    }
}
