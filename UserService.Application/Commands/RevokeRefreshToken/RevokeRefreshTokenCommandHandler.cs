using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using UserService.Infrastructure.Services;

namespace UserService.Application.Commands.RevokeRefreshToken
{
    public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, bool>
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
                _logger.LogWarning("No refresh token provided to revoke.");
                return false;
            }
            _logger.LogInformation("Attempting to revoke refresh token: {Token}", request.RefreshToken);

            await _tokenService.RevokeRefreshToken(request.RefreshToken);

            _logger.LogInformation("Refresh token revoked successfully: {Token}", request.RefreshToken);

            return true;

        }
    }
}
