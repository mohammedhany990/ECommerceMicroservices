using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UpdateOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(
            IRepository<Order> repository,
            IMapper mapper,
            CartServiceRpcClient cartServiceRpcClient,
            ShippingServiceRpcClient shippingServiceRpcClient,
            PaymentServiceRpcClient paymentServiceRpcClient,
            ILogger<UpdateOrderCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _cartServiceRpcClient = cartServiceRpcClient;
            _shippingServiceRpcClient = shippingServiceRpcClient;
            _paymentServiceRpcClient = paymentServiceRpcClient;
            _logger = logger;
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

            if (request.ShippingMethodId.HasValue)
            {
                _logger.LogInformation("Fetching shipping method {ShippingMethodId} for OrderId {OrderId}",
                    request.ShippingMethodId.Value, order.Id);

                var shippingMethod = await _shippingServiceRpcClient.GetShippingMethodByIdAsync(request.ShippingMethodId.Value);

                if (shippingMethod is not null)
                {
                    _logger.LogInformation("Shipping method found: {@ShippingMethod}", shippingMethod);
                    order.ShippingMethodId = shippingMethod.Id;
                    order.ShippingCost = shippingMethod.Cost;
                    order.ExpectedDeliveryDate = DateTime.UtcNow.AddDays(shippingMethod.EstimatedDeliveryDays);
                }
                else
                {
                    _logger.LogWarning("Shipping method {ShippingMethodId} not found", request.ShippingMethodId.Value);
                }
            }

            if (request.ShippingMethodId.HasValue || request.ShippingAddressId.HasValue)
            {
                _logger.LogInformation(
                    "Recalculating shipping cost for OrderId {OrderId} using ShippingAddressId {AddressId} and ShippingMethodId {MethodId}",
                    order.Id,
                    request.ShippingAddressId ?? order.ShippingAddressId,
                    request.ShippingMethodId ?? order.ShippingMethodId
                );

                var shippingResult = await _shippingServiceRpcClient.CalculateShippingCostAsync(
                    new Shared.DTOs.ShippingCostRequestDto(
                        request.ShippingAddressId ?? order.ShippingAddressId,
                        request.ShippingMethodId ?? order.ShippingMethodId
                    ));

                if (shippingResult is not null)
                {
                    _logger.LogInformation("Shipping recalculated: {@ShippingResult}", shippingResult);
                    order.ShippingCost = shippingResult.Cost;
                    order.TotalAmount = order.Subtotal + shippingResult.Cost;
                    order.ExpectedDeliveryDate = DateTime.UtcNow.AddDays(shippingResult.EstimatedDeliveryDays);
                }
                else
                {
                    _logger.LogWarning("Failed to recalculate shipping for OrderId {OrderId}", order.Id);
                }
            }

            order.UpdatedAt = DateTime.UtcNow;

            if (order.PaymentId.HasValue)
            {
                _logger.LogInformation("Fetching payment status for OrderId {OrderId}", order.Id);

                var payment = await _paymentServiceRpcClient.GetPaymentStatusAsync(order.Id);

                if (payment is null)
                {
                    _logger.LogError("Payment status retrieval failed for OrderId {OrderId}", order.Id);
                    throw new Exception("Failed to retrieve payment status.");
                }

                _logger.LogInformation("Payment service returned status {@PaymentStatus}", payment);

                if (Enum.TryParse<PaymentStatus>(payment.Status, true, out var parsedStatus))
                {
                    _logger.LogInformation("Parsed payment status for OrderId {OrderId}: {ParsedStatus}", order.Id, parsedStatus);
                    order.PaymentStatus = parsedStatus;
                }
                else
                {
                    _logger.LogWarning("Unknown payment status '{Status}' for OrderId {OrderId}. Setting to Pending.", payment.Status, order.Id);
                    order.PaymentStatus = PaymentStatus.Pending;
                }
            }

            if (request.ShippingAddressId.HasValue)
            {
                _logger.LogInformation("Updating ShippingAddressId for OrderId {OrderId} to {AddressId}",
                    order.Id, request.ShippingAddressId.Value);

                order.ShippingAddressId = request.ShippingAddressId.Value;
            }

            if (request.ExpectedDeliveryDate.HasValue)
            {
                _logger.LogInformation("Updating ExpectedDeliveryDate for OrderId {OrderId}", order.Id);
                order.ExpectedDeliveryDate = request.ExpectedDeliveryDate.Value;
            }

            if (order.Status == OrderStatus.Cancelled && order.Items.Any())
            {
                _logger.LogInformation("Order {OrderId} cancelled. Restoring {Count} items to cart.", order.Id, order.Items.Count);

                var cartItems = order.Items.Select(i => new Shared.DTOs.CartItemDto
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

            _logger.LogInformation("Order {OrderId} updated successfully. Result: {@OrderDto}", order.Id, result);

            return result;
        }
    }
}
