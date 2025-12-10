using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IRepository<Order> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;

        public GetOrderByIdQueryHandler(
            IRepository<Order> repository,
            IMapper mapper,
            ILogger<GetOrderByIdQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting GetOrderById query for OrderId {OrderId}", request.OrderId);

            if (request.OrderId == Guid.Empty)
            {
                _logger.LogWarning("Invalid OrderId provided (empty GUID)");
                throw new ArgumentException("OrderId cannot be empty", nameof(request.OrderId));
            }

            var order = await _repository.FirstOrDefaultAsync(
                x => x.Id == request.OrderId,
                include: x => x.Include(i => i.Items));

            if (order == null)
            {
                _logger.LogWarning("OrderId {OrderId} not found", request.OrderId);
                return null;
            }

            _logger.LogInformation("OrderId {OrderId} found with {ItemCount} items", request.OrderId, order.Items.Count);

            var dto = _mapper.Map<OrderDto>(order);

            _logger.LogInformation("Returning OrderDto for OrderId {OrderId}: {@OrderDto}", request.OrderId, dto);

            return dto;
        }
    }
}
