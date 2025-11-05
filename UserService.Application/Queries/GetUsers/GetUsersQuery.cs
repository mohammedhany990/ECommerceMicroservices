using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.DTOs;

namespace UserService.Application.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<List<UserDto>>
    {
    }
}
