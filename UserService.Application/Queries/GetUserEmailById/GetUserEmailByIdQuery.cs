using MediatR;

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
