using System.Security.Claims;
using UserService.Domain.Entities;
namespace UserService.Infrastructure.Services
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user);
        Task<RefreshToken> GenerateRefreshToken(Guid userId, string jwtId);
        Task<bool> ValidateRefreshToken(string refreshToken);
        Task<bool> RevokeRefreshToken(string refreshToken);
        Task RevokeAllUserRefreshTokens(Guid userId);
        Task<TokenResponse> UseRefreshToken(string refreshToken);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    }
}
