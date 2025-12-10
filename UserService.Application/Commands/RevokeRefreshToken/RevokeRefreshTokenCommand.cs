using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Commands.RevokeRefreshToken
{
    public class RevokeRefreshTokenCommand : IRequest<bool>
    {
        public string RefreshToken { get; set; }
        public RevokeRefreshTokenCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
