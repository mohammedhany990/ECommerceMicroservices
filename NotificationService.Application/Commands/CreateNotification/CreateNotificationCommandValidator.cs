using FluentValidation;

namespace NotificationService.Application.Commands.CreateNotification
{

    public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
    {
        public CreateNotificationCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.To)
                .NotEmpty().WithMessage("Recipient email is required.")
                .EmailAddress().WithMessage("To must be a valid email address.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(255).WithMessage("Subject cannot exceed 255 characters.");

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Body is required.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Type is required.")
                .MaximumLength(50).WithMessage("Type cannot exceed 50 characters.");
        }
    }
}

