using MediatR;
using UserService.Application.DTOs;

namespace UserService.Application.Queries.GetUsers
{
    public class GetUsersQuery : IRequest<List<UserDto>>
    {
    }
}
