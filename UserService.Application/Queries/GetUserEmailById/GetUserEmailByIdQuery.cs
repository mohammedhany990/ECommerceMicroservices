using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Queries.GetUserEmailById
{
    public class GetUserEmailByIdQuery : IRequest<string>
    {
        public Guid UserId { get; set; }
        public GetUserEmailByIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
