using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;
using ShippingService.Infrastructure.Messaging;
using ShippingService.Infrastructure.Services;

namespace ShippingService.Application.Commands.Shipments.CreateShipment
{
    public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, ShipmentDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Shipment> _repository;
        private readonly OrderServiceRpcClient _orderServiceRpcClient;
        private readonly IShippingCostCalculator _shippingCostCalculator;
        private readonly ILogger<CreateShipmentCommandHandler> _logger;
        private readonly PaymentServiceRpcClient _paymentServiceRpcClient;

        public CreateShipmentCommandHandler(
            IMapper mapper,
            IRepository<Shipment> repository,
            OrderServiceRpcClient orderServiceRpcClient,
            IShippingCostCalculator shippingCostCalculator,
            ILogger<CreateShipmentCommandHandler> logger,
            PaymentServiceRpcClient paymentServiceRpcClient) 
        {
            _mapper = mapper;
            _repository = repository;
            _orderServiceRpcClient = orderServiceRpcClient;
            _shippingCostCalculator = shippingCostCalculator;
            _logger = logger;
            _paymentServiceRpcClient = paymentServiceRpcClient;
        }

        public async Task<ShipmentDto> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating shipment for OrderId {OrderId}", request.OrderId);

            var orderDto = await _orderServiceRpcClient.GetOrderByIdAsync(request.OrderId);
            if (orderDto == null)
            {
                _logger.LogWarning("Order with Id {OrderId} not found", request.OrderId);
                throw new Exception("Order not found");
            }

            var paymentResultDto = await _paymentServiceRpcClient.GetPaymentAsync(request.OrderId);



            if (!Enum.TryParse<PaymentStatus>(orderDto.PaymentStatus, out var paymentStatus)
                || paymentStatus != PaymentStatus.Paid)
            {
                throw new InvalidOperationException("Order must be paid before shipping");
            }



            var shipmentExists = await _repository.AnyAsync(s => s.OrderId == request.OrderId, cancellationToken);
            if (shipmentExists)
            {
                _logger.LogWarning("Shipment already exists for OrderId {OrderId}", request.OrderId);
                throw new Exception("Shipment already exists for this order");
            }

            var shippingCost = await _shippingCostCalculator.CalculateAsync(request.ShippingAddressId, request.ShippingMethodId);
            _logger.LogInformation("Shipping cost for OrderId {OrderId} calculated: {Cost}", request.OrderId, shippingCost.Cost);

           

            var shipment = _mapper.Map<Shipment>(request);

            shipment.ShippingCost = shippingCost.Cost;
            shipment.TrackingNumber = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpperInvariant();
            shipment.Status = ShipmentStatus.Pending;
            shipment.CreatedAt = DateTime.UtcNow;
            shipment.UpdatedAt = DateTime.UtcNow;

            await _repository.AddAsync(shipment);
            await _repository.SaveChangesAsync();


            try
            {
                var updated = await _orderServiceRpcClient.UpdateOrderStatusAsync(shipment.OrderId, nameof(OrderStatus.Shipped));
                _logger.LogInformation(updated
                    ? "Order status updated to Shipped for OrderId {OrderId}"
                    : "Failed to update order status for OrderId {OrderId}", shipment.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for OrderId {OrderId}", shipment.OrderId);
            }

            _logger.LogInformation("Shipment created successfully for OrderId {OrderId} with ShipmentId {ShipmentId}", request.OrderId, shipment.Id);

            return _mapper.Map<ShipmentDto>(shipment);
        }
    }
}
