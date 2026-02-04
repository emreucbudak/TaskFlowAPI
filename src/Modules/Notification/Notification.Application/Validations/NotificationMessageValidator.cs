using FluentValidation;
using Notification.Domain.Models;

namespace Notification.Application.Validations
{
    public class NotificationMessageValidator : AbstractValidator<NotificationMessage>
    {
        public NotificationMessageValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Bildirim başlığı boş olamaz.")
                .MaximumLength(100).WithMessage("Bildirim başlığı 100 karakterden fazla olamaz.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Bildirim açıklaması boş olamaz.")
                .MaximumLength(500).WithMessage("Bildirim açıklaması 500 karakterden fazla olamaz.");

            RuleFor(x => x.ReceiverUserId)
                .NotEmpty().WithMessage("Alıcı kullanıcı bilgisi boş olamaz.");

            RuleFor(x => x.SendTime)
                .NotEmpty().WithMessage("Gönderim zamanı belirtilmelidir.");
        }
    }
}
