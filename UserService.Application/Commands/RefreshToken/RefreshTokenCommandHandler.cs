using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Services;

namespace UserService.Application.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IRepository<User> _repository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            IRepository<User> repository,
            ITokenService tokenService,
            IMapper mapper,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _repository = repository;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting refresh token process.");

            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                _logger.LogWarning("Invalid access token provided during refresh.");
                throw new UnauthorizedAccessException("Invalid access token.");
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("Access token missing NameIdentifier claim.");
                throw new UnauthorizedAccessException("Invalid token claims.");
            }

            _logger.LogInformation("Extracted UserId {UserId} from expired token.", userIdClaim);

            var tokenResponse = await _tokenService.UseRefreshToken(request.RefreshToken);
            if (tokenResponse == null)
            {
                _logger.LogWarning("Invalid or expired refresh token for UserId {UserId}", userIdClaim);
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            var user = await _repository.GetByIdAsync(Guid.Parse(userIdClaim));
            if (user == null)
            {
                _logger.LogWarning("User not found with Id {UserId}", userIdClaim);
                throw new UnauthorizedAccessException("User not found.");
            }

            var userDto = _mapper.Map<UserDto>(user);

            _logger.LogInformation("Refresh token rotated successfully for UserId {UserId}", userIdClaim);

            return new AuthResponseDto
            {
                User = userDto,
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken
            };
        }
    }
}