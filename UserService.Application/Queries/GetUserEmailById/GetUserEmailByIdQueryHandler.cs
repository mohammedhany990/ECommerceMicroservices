using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Queries.GetUserEmailById
{
    public class GetUserEmailByIdQueryHandler : IRequestHandler<GetUserEmailByIdQuery, string>
    {
        private readonly IRepository<User> _repository;

        public GetUserEmailByIdQueryHandler(IRepository<User> repository)
        {
            _repository = repository;
        }
        public async Task<string> Handle(GetUserEmailByIdQuery request, CancellationToken cancellationToken)
        {
            if(request.UserId == Guid.Empty)
            {
                throw new ArgumentException("UserId cannot be empty", nameof(request.UserId));
            }

            var user = await _repository.GetByIdAsync(request.UserId);
            if(user == null)
            {
                throw new KeyNotFoundException($"User with Id {request.UserId} not found");
            }

            return user.Email;
        }
    }
}
