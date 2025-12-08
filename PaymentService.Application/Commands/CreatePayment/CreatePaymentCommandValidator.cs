using FluentValidation;

namespace PaymentService.Application.Commands.CreatePayment
{
    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("OrderId is required.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");




        }
    }
}
