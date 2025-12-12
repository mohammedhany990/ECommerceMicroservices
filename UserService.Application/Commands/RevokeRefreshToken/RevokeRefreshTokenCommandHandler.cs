using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using UserService.Infrastructure.Services;

namespace UserService.Application.Commands.RevokeRefreshToken
{
    public class RevokeRefreshTokenCommandHandler
        : IRequestHandler<RevokeRefreshTokenCommand, bool>
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<RevokeRefreshTokenCommandHandler> _logger;

        public RevokeRefreshTokenCommandHandler(
            ITokenService tokenService,
            ILogger<RevokeRefreshTokenCommandHandler> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<bool> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                _logger.LogWarning("Revoke refresh token failed: empty token provided.");
                return false;
            }

            _logger.LogInformation("Attempting to revoke refresh token: {Token}", request.RefreshToken);

            var revoked = await _tokenService.RevokeRefreshToken(request.RefreshToken);

            if (!revoked)
            {
                _logger.LogWarning("Failed to revoke refresh token: {Token}", request.RefreshToken);
                return false;
            }

            _logger.LogInformation("Refresh token revoked successfully: {Token}", request.RefreshToken);
            return true;
        }
    }
}
