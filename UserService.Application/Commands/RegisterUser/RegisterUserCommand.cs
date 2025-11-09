using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Commands.RegisterUser
{
    public class RegisterUserCommand : IRequest<AuthResponseDto>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
    }
}
