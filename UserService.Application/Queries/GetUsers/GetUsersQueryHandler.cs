using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Queries.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        private readonly IRepository<User> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUsersQueryHandler> _logger;

        public GetUsersQueryHandler(
            IRepository<User> repository,
            IMapper mapper,
            ILogger<GetUsersQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting GetUsersQuery...");

            var users = await _repository.GetAllAsync();

            _logger.LogInformation("Fetched {Count} users from the database.", users.Count());

            var userDtos = _mapper.Map<List<UserDto>>(users);

            _logger.LogInformation("Mapped users to UserDto list.");

            return userDtos;
        }
    }
}
