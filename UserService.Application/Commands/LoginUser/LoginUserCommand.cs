using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Commands.LoginUser
{
    public class LoginUserCommand : IRequest<AuthResponseDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
