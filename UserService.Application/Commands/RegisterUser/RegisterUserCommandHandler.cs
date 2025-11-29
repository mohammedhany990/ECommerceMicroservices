using AutoMapper;
using MediatR;
using Shared.Messaging;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponseDto>
    {
        private readonly IRepository<User> _repository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRabbitMqPublisher<CreateNotificationEvent> _publisher;

        public RegisterUserCommandHandler(IRepository<User> repository,
            IMapper mapper,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IRabbitMqPublisher<CreateNotificationEvent> publisher)
        {
            _repository = repository;
            _mapper = mapper;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _publisher = publisher;
        }
        public async Task<AuthResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _repository.FirstOrDefaultAsync(e => e.Email == request.Email);
            if (existingUser is not null)
                throw new Exception("Email already exists");

            var hashedPassword = _passwordHasher.HashPassword(request.Password);

            var user = _mapper.Map<User>(request);
            user.PasswordHash = hashedPassword;


            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _repository.SaveChangesAsync();

            var mappedUser = _mapper.Map<UserDto>(user);

            var notificationEvent = new CreateNotificationEvent
            {
                UserId = user.Id,
                To = user.Email,
                Subject = "Welcome to Our Service!",
                Body = $"Hello {user.Username},\n\nThank you for registering with our service!"
            };

            _publisher.Publish(notificationEvent);

            return new AuthResponseDto
            {
                User = mappedUser,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }
    }
}
