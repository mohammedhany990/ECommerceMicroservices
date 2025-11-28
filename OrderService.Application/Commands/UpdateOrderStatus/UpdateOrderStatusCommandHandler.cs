using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Interfaces;
using OrderService.Infrastructure.Services;
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
        private readonly IRabbitMqPublisher<CreateNotificationEvent> _rabbitMqPublisher;
        private readonly UserServiceClient _userServiceClient;

        public UpdateOrderStatusCommandHandler(
            IRepository<Order> repository,
            IMapper mapper,
            IRabbitMqPublisher<CreateNotificationEvent> rabbitMqPublisher,
            UserServiceClient userServiceClient
            )
        {
            _repository = repository;
            _mapper = mapper;
            _rabbitMqPublisher = rabbitMqPublisher;
            _userServiceClient = userServiceClient;
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


            
            if (order.Status == OrderStatus.Cancelled)
            {

                var userEmail = await _userServiceClient.GetUserEmailAsync(order.UserId);
                var notificationEvent = new CreateNotificationEvent
                {
                    UserId = order.UserId,
                    To= userEmail,
                    Subject = "Order Cancelled",
                    Body = $@"
                                Hello,

                                Your order with ID {order.Id} has been cancelled.

                                Cancelled At: {order.UpdatedAt:yyyy-MM-dd HH:mm:ss} UTC

                                If you did not request this cancellation, please contact support immediately.

                                Thank you,
                                Ecommerce
                                "

                };
                _ = Task.Run(() => _rabbitMqPublisher.Publish(notificationEvent));

            }

            return _mapper.Map<OrderDto>(order);
        }
    }
}
