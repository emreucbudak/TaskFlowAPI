using Chat.Domain.Entities;
using FluentValidation;

namespace Chat.Application.Validations
{
    public class MessageValidator : AbstractValidator<Message>
    {
        public MessageValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Mesaj içeriği boş olamaz.")
                .MaximumLength(2000).WithMessage("Mesaj içeriği 2000 karakterden fazla olamaz.");

            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("Gönderen bilgisi boş olamaz.");

            RuleFor(x => x)
                .Must(x => x.ReceiverId != Guid.Empty || (x.GroupId != null && x.GroupId != Guid.Empty))
                .WithMessage("Mesajın bir alıcısı veya grubu olmalıdır.");

            RuleFor(x => x.SendTime)
                .NotEmpty().WithMessage("Gönderim zamanı belirtilmelidir.");
        }
    }
}
