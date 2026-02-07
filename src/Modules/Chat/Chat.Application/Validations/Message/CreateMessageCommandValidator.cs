using Chat.Application.Features.CQRS.Message.Command.Create;
using FluentValidation;

namespace Chat.Application.Validations.Message
{
    public class CreateMessageCommandValidator : AbstractValidator<CreateMessageCommandRequest>
    {
        public CreateMessageCommandValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Message content cannot be empty.")
                .MaximumLength(1000).WithMessage("Message content cannot exceed 1000 characters.");

            RuleFor(x => x.SenderId)
                .NotEmpty().WithMessage("Sender ID is required.");

            RuleFor(x => x)
                .Must(x => x.ReceiverId.HasValue || x.GroupId.HasValue)
                .WithMessage("Either ReceiverId or GroupId must be provided.");
        }
    }
}
