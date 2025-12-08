using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IRepository<Order> _repository;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(IRepository<Order> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.OrderId == Guid.Empty)
                throw new ArgumentException("OrderId cannot be empty", nameof(request.OrderId));


            var order = await _repository.FirstOrDefaultAsync(x => x.Id == request.OrderId, include: x => x.Include(i => i.Items));
            if (order == null)
                return null;

            return _mapper.Map<OrderDto>(order);
        }
    }
}
