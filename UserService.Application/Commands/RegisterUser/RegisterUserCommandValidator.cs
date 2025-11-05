using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure.Data;

namespace UserService.Application.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        private readonly AppDbContext _dbContext;

        public RegisterUserCommandValidator(AppDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long");

            RuleFor(u => u.Email)
           .NotEmpty().WithMessage("Email is required.")
           .EmailAddress().WithMessage("Invalid email format.")
           .MustAsync(async (email, cancellation) =>
               !await _dbContext.Users.AnyAsync(u => u.Email == email, cancellation)
           ).WithMessage("Email already exists.");


           

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Matches(@"^\+?\d{10,15}$").WithMessage("Invalid phone number");

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters");
        }
    }
}
