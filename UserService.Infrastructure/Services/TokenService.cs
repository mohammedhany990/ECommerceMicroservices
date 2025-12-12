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
        private readonly IRepository<RefreshToken> _refreshTokenRepository;
        private readonly IRepository<User> _userRepository;

        public TokenService(
            IOptions<JwtSettings> jwtOptions,
            IRepository<RefreshToken> refreshTokenRepository,
            IRepository<User> userRepository)
        {
            _jwtSettings = jwtOptions.Value;
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
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

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

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
            var tokens = await _refreshTokenRepository.GetAllAsync(x => x.UserId == userId);

            foreach (var rt in tokens)
            {
                rt.RevokedOn = DateTime.UtcNow;
            }

            await _refreshTokenRepository.SaveChangesAsync();
        }

        public async Task<bool> RevokeRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            var token = await _refreshTokenRepository
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (token == null || !token.IsActive)
                return false;

            token.RevokedOn = DateTime.UtcNow;

            await _refreshTokenRepository.UpdateAsync(token);
            await _refreshTokenRepository.SaveChangesAsync();

            return true;
        }


        public async Task<TokenResponse> UseRefreshToken(string refreshToken)
        {
            var oldToken = await _refreshTokenRepository.FirstOrDefaultAsync(x => x.Token == refreshToken);
            if (oldToken is null || !oldToken.IsActive)
                return null;

            oldToken.RevokedOn = DateTime.UtcNow;
            await _refreshTokenRepository.UpdateAsync(oldToken);


            var user = await _userRepository.GetByIdAsync(oldToken.UserId);
            if (user is null)
                return null;

            var newJwtId = Guid.NewGuid().ToString();

            var newAccessToken = await GenerateAccessToken(user);
            var newRefreshToken = await GenerateRefreshToken(oldToken.UserId, newJwtId);

            await _refreshTokenRepository.SaveChangesAsync();

            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };
        }





        public async Task<bool> ValidateRefreshToken(string refreshToken)
        {
            var token = await _refreshTokenRepository.FirstOrDefaultAsync(x => x.Token == refreshToken);

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