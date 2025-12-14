using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;
using Shared.DTOs;
using Shared.Enums;
using System;
using OrderDto = OrderService.Application.DTOs.OrderDto;
namespace OrderService.Application.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, OrderDto>
    {
        private readonly IRepository<Order> _repository;
        private readonly IMapper _mapper;
        private readonly CartServiceRpcClient _cartServiceRpcClient;
        private readonly ShippingServiceRpcClient _shippingServiceRpcClient;
        private readonly PaymentServiceRpcClient _paymentServiceRpcClient;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(
            IRepository<Order> repository,
            IMapper mapper,
            CartServiceRpcClient cartServiceRpcClient,
            ShippingServiceRpcClient shippingServiceRpcClient,
            PaymentServiceRpcClient paymentServiceRpcClient,
            ILogger<UpdateOrderCommandHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cartServiceRpcClient = cartServiceRpcClient ?? throw new ArgumentNullException(nameof(cartServiceRpcClient));
            _shippingServiceRpcClient = shippingServiceRpcClient ?? throw new ArgumentNullException(nameof(shippingServiceRpcClient));
            _paymentServiceRpcClient = paymentServiceRpcClient ?? throw new ArgumentNullException(nameof(paymentServiceRpcClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateOrder started for OrderId {OrderId} with Payload {@Request}", request.OrderId, request);

            var order = await _repository.FirstOrDefaultAsync(
                o => o.Id == request.OrderId,
                include: q => q.Include(o => o.Items)
            );

            if (order is null)
            {
                _logger.LogWarning("OrderId {OrderId} not found", request.OrderId);
                return null!;
            }

            _logger.LogInformation("Order {OrderId} found. Current status: {Status}", order.Id, order.Status);

            if (request.Status.HasValue)
            {
                _logger.LogInformation("Updating order status for {OrderId} to {NewStatus}", order.Id, request.Status);
                order.Status = request.Status.Value;
            }

            string methodName = "Unknown";

            if (request.ShippingMethodId.HasValue)
            {
                _logger.LogInformation("Fetching shipping method {ShippingMethodId} for OrderId {OrderId}",
                    request.ShippingMethodId.Value, order.Id);

                var shippingMethod = await _shippingServiceRpcClient.GetShippingMethodByIdAsync(request.ShippingMethodId.Value);



                if (shippingMethod != null)
                {
                    order.ShippingMethodId = shippingMethod.Id;
                    order.ShippingCost = shippingMethod.Cost;
                    order.ExpectedDeliveryDate = DateTime.UtcNow.AddDays(shippingMethod.EstimatedDeliveryDays);
                    methodName = shippingMethod.Name ?? "Unknown";

                    _logger.LogInformation("Shipping method found: {@ShippingMethod}", shippingMethod);
                }
            }

            if (request.ShippingMethodId.HasValue || request.ShippingAddressId.HasValue)
            {
                var shippingRequest = new ShippingCostRequestDto(
                    request.ShippingAddressId ?? order.ShippingAddressId,
                    request.ShippingMethodId ?? order.ShippingMethodId
                );

                _logger.LogInformation(
                    "Recalculating shipping cost for OrderId {OrderId} using ShippingAddressId {AddressId} and ShippingMethodId {MethodId}",
                    order.Id,
                    shippingRequest.ShippingAddressId,
                    shippingRequest.ShippingMethodId
                );

                var shippingResult = await _shippingServiceRpcClient.CalculateShippingCostAsync(shippingRequest);



                if (shippingResult != null)
                {
                    order.ShippingCost = shippingResult.Cost;
                    order.TotalAmount = order.Subtotal + shippingResult.Cost;
                    order.ExpectedDeliveryDate = DateTime.UtcNow.AddDays(shippingResult.EstimatedDeliveryDays);
                    order.ShippingMethodId = shippingResult.ShippingMethodId;
                    methodName = shippingResult.MethodName ?? methodName;

                    _logger.LogInformation("Shipping recalculated: {@ShippingResult}", shippingResult);
                }
            }

            order.UpdatedAt = DateTime.UtcNow;

            if (order.PaymentId.HasValue)
            {
                _logger.LogInformation("Fetching payment status for OrderId {OrderId}", order.Id);

                var payment = await _paymentServiceRpcClient.GetPaymentAsync(order.Id);

                if (payment != null)
                {

                    if (Enum.TryParse<PaymentStatus>(payment.Status, true, out var parsedStatus))
                    {
                        order.PaymentStatus = parsedStatus;
                    }
                    else
                    {
                        _logger.LogWarning("Unknown payment status '{Status}' for OrderId {OrderId}. Setting to Pending.", payment.Status, order.Id);
                        order.PaymentStatus = PaymentStatus.Pending;
                    }
                }
                else
                {
                    _logger.LogWarning("Payment status retrieval failed for OrderId {OrderId}", order.Id);
                    order.PaymentStatus = PaymentStatus.Pending;
                }
            }

            if (request.ShippingAddressId.HasValue)
            {
                order.ShippingAddressId = request.ShippingAddressId.Value;
                _logger.LogInformation("Updated ShippingAddressId for OrderId {OrderId} to {AddressId}", order.Id, order.ShippingAddressId);
            }

            if (request.ExpectedDeliveryDate.HasValue)
            {
                order.ExpectedDeliveryDate = request.ExpectedDeliveryDate.Value;
                _logger.LogInformation("Updated ExpectedDeliveryDate for OrderId {OrderId}", order.Id);
            }

            if (order.Status == OrderStatus.Cancelled && order.Items.Any())
            {
                _logger.LogInformation("Order {OrderId} cancelled. Restoring {Count} items to cart.", order.Id, order.Items.Count);

                var cartItems = order.Items.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList();

                await _cartServiceRpcClient.RestoreItemsToCart(order.UserId, cartItems);
            }

            _logger.LogInformation("Saving updated order {OrderId} to database", order.Id);
            await _repository.UpdateAsync(order);
            await _repository.SaveChangesAsync();

            var result = _mapper.Map<OrderDto>(order);
            result.ShippingMethod = methodName;

            _logger.LogInformation("Order {OrderId} updated successfully. Result: {@OrderDto}", order.Id, result);

            return result;
        }
    }
}
