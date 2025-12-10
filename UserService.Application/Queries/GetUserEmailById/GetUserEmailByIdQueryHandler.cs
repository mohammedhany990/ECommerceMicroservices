using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Queries.GetUserEmailById
{
    public class GetUserEmailByIdQueryHandler : IRequestHandler<GetUserEmailByIdQuery, string>
    {
        private readonly IRepository<User> _repository;
        private readonly ILogger<GetUserEmailByIdQueryHandler> _logger;

        public GetUserEmailByIdQueryHandler(
            IRepository<User> repository,
            ILogger<GetUserEmailByIdQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<string> Handle(GetUserEmailByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetUserEmailByIdQuery for UserId: {UserId}", request.UserId);

            if (request.UserId == Guid.Empty)
            {
                _logger.LogWarning("GetUserEmailByIdQuery received an empty UserId.");
                throw new ArgumentException("UserId cannot be empty", nameof(request.UserId));
            }

            var user = await _repository.GetByIdAsync(request.UserId);

            if (user == null)
            {
                _logger.LogWarning("User not found for UserId: {UserId}", request.UserId);
                throw new KeyNotFoundException($"User with Id {request.UserId} not found");
            }

            _logger.LogInformation("Successfully retrieved email for UserId: {UserId}", request.UserId);

            return user.Email;
        }
    }
}
