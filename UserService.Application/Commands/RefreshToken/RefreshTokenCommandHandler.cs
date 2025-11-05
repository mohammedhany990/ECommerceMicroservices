using MediatR;
using System.Security.Claims;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using System.Security.Claims;
using AutoMapper;

namespace UserService.Application.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IRepository<User> _repository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public RefreshTokenCommandHandler(IRepository<User> repository, ITokenService tokenService, IMapper mapper)
        {
            _repository = repository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal is null)
                throw new UnauthorizedAccessException("Invalid access token.");

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException("Invalid token claims.");

            var user = await _repository.FirstOrDefaultAsync(e=> e.Email == email);
            if (user is null)
                throw new UnauthorizedAccessException("User not found.");

            if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _repository.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);

            return new AuthResponseDto
            {
                User = userDto,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
    }

}
