using Chat.Application.Features.CQRS.Message.Command.Update;
using FluentValidation;

namespace Chat.Application.Validations.Message
{
    public class UpdateMessageCommandValidator : AbstractValidator<UpdateMessageCommandRequest>
    {
        public UpdateMessageCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Message ID is required.");

            RuleFor(x => x.NewContent)
                .NotEmpty().WithMessage("New content cannot be empty.")
                .MaximumLength(1000).WithMessage("New content cannot exceed 1000 characters.");
        }
    }
}
