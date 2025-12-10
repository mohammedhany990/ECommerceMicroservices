using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Messaging;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Services;

namespace UserService.Application.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRabbitMqPublisher<CreateNotificationEvent> _publisher;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(
            IRepository<User> userRepository,
            IMapper mapper,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IRabbitMqPublisher<CreateNotificationEvent> publisher,
            ILogger<RegisterUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _publisher = publisher;
            _logger = logger;
        }

        public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower());
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed. Email already exists: {Email}", request.Email);
                throw new Exception("Email already exists");
            }

            var user = _mapper.Map<User>(request);
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);

            var jwtId = Guid.NewGuid().ToString();
            var accessToken =await _tokenService.GenerateAccessToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken(user.Id, jwtId);

            _logger.LogInformation("Access and refresh tokens generated for UserID: {UserId}", user.Id);

            var mappedUser = _mapper.Map<UserDto>(user);

            var notificationEvent = new CreateNotificationEvent
            {
                UserId = user.Id,
                To = user.Email,
                Subject = "Welcome to Our Service!",
                Body = $"Hello {user.Username},\n\nThank you for registering with our service!"
            };
            _publisher.Publish(notificationEvent);
            _logger.LogInformation("Notification event published for UserID: {UserId}", user.Id);

            _logger.LogInformation("Registration completed successfully for UserID: {UserId}", user.Id);

            return new AuthResponseDto
            {
                User = mappedUser,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
