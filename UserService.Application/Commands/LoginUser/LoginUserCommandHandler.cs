using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Services;

namespace UserService.Application.Commands.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthResponseDto>
    {
        private readonly IRepository<User> _repository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<LoginUserCommandHandler> _logger;

        public LoginUserCommandHandler(
            IRepository<User> repository,
            IMapper mapper,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            ILogger<LoginUserCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<AuthResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for Email {Email}", request.Email);

            var normalizedEmail = request.Email.Trim().ToLower();
            var user = await _repository.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (user == null)
            {
                _logger.LogWarning("Login failed: user not found for Email {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: incorrect password for Email {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            _logger.LogInformation("User {UserId} authenticated successfully", user.Id);

            var jwtId = Guid.NewGuid().ToString();

            var accessToken = await _tokenService.GenerateAccessToken(user);

            var refreshToken = await _tokenService.GenerateRefreshToken(user.Id, jwtId);

            _logger.LogInformation("Access and refresh tokens generated for UserId {UserId}", user.Id);

            var userDto = _mapper.Map<UserDto>(user);

            _logger.LogInformation("LoginUserCommand completed for UserId {UserId}", user.Id);

            return new AuthResponseDto
            {
                User = userDto,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}