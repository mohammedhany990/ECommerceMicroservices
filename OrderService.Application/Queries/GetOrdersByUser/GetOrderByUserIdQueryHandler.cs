using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Queries.GetOrdersByUser
{
    public class GetOrderByUserIdQueryHandler : IRequestHandler<GetOrderByUserIdQuery, OrderDto>
    {
        private readonly IRepository<Order> _repository;
        private readonly IMapper _mapper;

        public GetOrderByUserIdQueryHandler(IRepository<Order> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<OrderDto> Handle(GetOrderByUserIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _repository.FirstOrDefaultAsync(x => x.UserId == request.UserId,
                include: q => q.Include(o => o.Items));

            if (order is null)
            {
                throw new KeyNotFoundException("User with the specified ID was not found.");
            }

            var orderDto = _mapper.Map<OrderDto>(order);

            return orderDto;
        }
    }
}
