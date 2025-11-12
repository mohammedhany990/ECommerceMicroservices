using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Services;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderDto>
    {
        private readonly IRepository<Order> _repository;
        private readonly IMapper _mapper;
        private readonly CartServiceClient _cartServiceClient;
        private readonly ShippingServiceClient _shippingServiceClient;

        public UpdateOrderCommandHandler(IRepository<Order> repository,
            IMapper mapper,
            CartServiceClient cartServiceClient,
            ShippingServiceClient shippingServiceClient)
        {
            _repository = repository;
            _mapper = mapper;
            _cartServiceClient = cartServiceClient;
            _shippingServiceClient = shippingServiceClient;
        }

        public async Task<OrderDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.FirstOrDefaultAsync(
                o => o.Id == request.OrderId,
                include: q => q.Include(o => o.Items)
            );

            if (order is null)
                return null!;

            if (request.Status.HasValue)
                order.Status = request.Status.Value;

            if (request.ShippingMethodId.HasValue)
            {
                var shippingMethod = await _shippingServiceClient.GetShippingMethodByIdAsync(request.ShippingMethodId.Value);
                if (shippingMethod != null)
                {
                    order.ShippingMethod = shippingMethod.Name;
                    order.ShippingCost = shippingMethod.Cost;
                    order.ExpectedDeliveryDate = DateTime.UtcNow.AddDays(shippingMethod.EstimatedDeliveryDays);
                }
            }

            if (!string.IsNullOrEmpty(request.PaymentMethod))
                order.PaymentMethod = request.PaymentMethod;

            if (request.ShippingAddressId.HasValue)
                order.ShippingAddressId = request.ShippingAddressId.Value;

            if (request.ExpectedDeliveryDate.HasValue)
                order.ExpectedDeliveryDate = request.ExpectedDeliveryDate.Value;

            order.UpdatedAt = DateTime.UtcNow;

            if (order.Status == OrderStatus.Cancelled && order.Items.Any() && !string.IsNullOrEmpty(request.AuthToken))
            {
                var cartItems = order.Items.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList();
                await _cartServiceClient.RestoreItemsToCart(request.AuthToken, cartItems);
               
            }

            await _repository.UpdateAsync(order);
            await _repository.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

    }

}
