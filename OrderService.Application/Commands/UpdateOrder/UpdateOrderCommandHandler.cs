using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;


namespace OrderService.Application.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderDto>
    {
        private readonly IRepository<Order> _repository;
        private readonly IMapper _mapper;
        private readonly CartServiceRpcClient _cartServiceRpcClient;
        private readonly ShippingServiceRpcClient _shippingServiceRpcClient;
        private readonly PaymentServiceRpcClient _paymentServiceRpcClient;

        public UpdateOrderCommandHandler(IRepository<Order> repository,
            IMapper mapper,
            CartServiceRpcClient cartServiceRpcClient,
            ShippingServiceRpcClient shippingServiceRpcClient,
            PaymentServiceRpcClient paymentServiceRpcClient)
        {
            _repository = repository;
            _mapper = mapper;
            _cartServiceRpcClient = cartServiceRpcClient;
            _shippingServiceRpcClient = shippingServiceRpcClient;
            _paymentServiceRpcClient = paymentServiceRpcClient;
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
                var shippingMethod = await _shippingServiceRpcClient.GetShippingMethodByIdAsync(request.ShippingMethodId.Value);
                if (shippingMethod is not null)
                {
                    order.ShippingMethodId = shippingMethod.Id;
                    order.ShippingCost = shippingMethod.Cost;
                    order.ExpectedDeliveryDate = DateTime.UtcNow.AddDays(shippingMethod.EstimatedDeliveryDays);
                }

            }
            if (request.ShippingMethodId.HasValue || request.ShippingAddressId.HasValue)
            {
                var shippingResult = await _shippingServiceRpcClient.CalculateShippingCostAsync(
                    new Shared.DTOs.ShippingCostRequestDto(
                        request.ShippingAddressId ?? order.ShippingAddressId,
                        request.ShippingMethodId ?? order.ShippingMethodId));

                if (shippingResult is not null)
                {
                    order.ShippingCost = shippingResult.Cost;
                    order.TotalAmount = order.Subtotal + shippingResult.Cost;
                    order.ExpectedDeliveryDate = DateTime.UtcNow.AddDays(shippingResult.EstimatedDeliveryDays);
                }
            }

            order.UpdatedAt = DateTime.UtcNow;

            if (order.PaymentId.HasValue)
            {
                var payment = await _paymentServiceRpcClient.GetPaymentStatusAsync(order.Id);

                if (payment is null)
                {
                    throw new Exception("Failed to retrieve payment status.");
                }

                if (Enum.TryParse<PaymentStatus>(payment.Status, true, out var parsedStatus))
                {
                    order.PaymentStatus = parsedStatus;
                }
                else
                {
                    order.PaymentStatus = PaymentStatus.Pending;                
                }
            }


            if (request.ShippingAddressId.HasValue)
                order.ShippingAddressId = request.ShippingAddressId.Value;

            if (request.ExpectedDeliveryDate.HasValue)
                order.ExpectedDeliveryDate = request.ExpectedDeliveryDate.Value;


            if (order.Status == OrderStatus.Cancelled && order.Items.Any())
            {
                var cartItems = order.Items.Select(i => new Shared.DTOs.CartItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList();
                await _cartServiceRpcClient.RestoreItemsToCart(order.UserId, cartItems);

            }

            await _repository.UpdateAsync(order);
            await _repository.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

    }

}
