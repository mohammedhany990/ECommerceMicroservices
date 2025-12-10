using Consul;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Configurations;

namespace UserService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IRepository<RefreshToken> _repository;

        public TokenService(IOptions<JwtSettings> jwtOptions, IRepository<RefreshToken> repository)
        {
            _jwtSettings = jwtOptions.Value;
            _repository = repository;
        }
        public async Task<string> GenerateAccessToken(User user)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenDurationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<RefreshToken> GenerateRefreshToken(Guid userId, string jwtId)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            var tokenString = Convert.ToBase64String(randomBytes);

            var refreshToken = new RefreshToken
            {
                Token = tokenString,
                JwtId = jwtId,
                UserId = userId,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDurationDays)
            };

            await _repository.AddAsync(refreshToken);
            await _repository.SaveChangesAsync();

            return refreshToken;
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtSettings.Key)
                ),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is JwtSecurityToken jwt &&
                    jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public async Task RevokeAllUserRefreshTokens(Guid userId)
        {
            var tokens = await _repository.GetAllAsync(x => x.UserId == userId);

            foreach (var rt in tokens)
            {
                rt.RevokedOn = DateTime.UtcNow;
            }

            await _repository.SaveChangesAsync();
        }

        public async Task RevokeRefreshToken(string refreshToken)
        {
            var token = await _repository.FirstOrDefaultAsync(x => x.Token == refreshToken);
            if (token == null) return;

            token.RevokedOn = DateTime.UtcNow;

            await _repository.UpdateAsync(token);
            await _repository.SaveChangesAsync();
        }

        public async Task<TokenResponse> UseRefreshToken(string refreshToken)
        {
            var oldToken = await _repository.FirstOrDefaultAsync(x => x.Token == refreshToken);
            if (oldToken == null || !oldToken.IsActive)
                return null;

            oldToken.RevokedOn = DateTime.UtcNow;
            await _repository.UpdateAsync(oldToken);

            var newJwtId = Guid.NewGuid().ToString();

            var newAccessToken = GenerateNewAccessToken(oldToken.UserId.ToString(), newJwtId);
            var newRefreshToken = await GenerateRefreshToken(oldToken.UserId, newJwtId);

            await _repository.SaveChangesAsync();

            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }


        private string GenerateNewAccessToken(string userId, string jwtId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(JwtRegisteredClaimNames.Jti, jwtId)
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenDurationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public async Task<bool> ValidateRefreshToken(string refreshToken)
        {
            var token = await _repository.FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (token == null) return false;
            if (!token.IsActive) return false;

            return true;
        }

       
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }

}