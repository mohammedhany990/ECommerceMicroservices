using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, OrderDto>
    {
        private readonly IRepository<Order> _repository;
        private readonly IMapper _mapper;

        public UpdateOrderStatusCommandHandler(IRepository<Order> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<OrderDto> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.FirstOrDefaultAsync(o => o.Id == request.OrderId,
                include: q => q.Include(i => i.Items));

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            order.Status = request.NewStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(order);

            await _repository.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }
    }
}
