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
    public class GetOrderByUserIdQueryHandler : IRequestHandler<GetOrderByUserIdQuery, IReadOnlyList<OrderDto>>
    {
        private readonly IRepository<Order> _repository;
        private readonly IMapper _mapper;

        public GetOrderByUserIdQueryHandler(IRepository<Order> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IReadOnlyList<OrderDto>> Handle(GetOrderByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userOrders = await _repository.GetAllAsync(x => x.UserId == request.UserId,
                include: q=>q.Include(i=>i.Items));

            if (!userOrders.Any())
                return new List<OrderDto>();

            var orderDtos = _mapper.Map<IReadOnlyList<OrderDto>>(userOrders);
            return orderDtos;
        }
    }
}
