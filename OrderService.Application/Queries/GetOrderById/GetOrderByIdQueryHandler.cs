using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;
using Shared.Enums;

namespace OrderService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IRepository<Order> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOrderByIdQueryHandler> _logger;
        private readonly ShippingServiceRpcClient _shippingServiceRpcClient;
        private readonly PaymentServiceRpcClient _paymentServiceRpcClient;

        public GetOrderByIdQueryHandler(
            IRepository<Order> repository,
            IMapper mapper,
            ILogger<GetOrderByIdQueryHandler> logger,
            ShippingServiceRpcClient shippingServiceRpcClient,
            PaymentServiceRpcClient paymentServiceRpcClient)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _shippingServiceRpcClient = shippingServiceRpcClient;
            _paymentServiceRpcClient = paymentServiceRpcClient;
        }

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting GetOrderById for OrderId {OrderId}", request.OrderId);

            if (request.OrderId == Guid.Empty)
            {
                _logger.LogWarning("Invalid OrderId provided (empty GUID)");
                throw new ArgumentException("OrderId cannot be empty", nameof(request.OrderId));
            }

            var order = await _repository.FirstOrDefaultAsync(
                x => x.Id == request.OrderId,
                include: x => x.Include(i => i.Items)
            );

            if (order == null)
            {
                _logger.LogWarning("OrderId {OrderId} not found", request.OrderId);
                return null;
            }

            _logger.LogInformation("OrderId {OrderId} found with {ItemCount} items", request.OrderId, order.Items.Count);

            var dto = _mapper.Map<OrderDto>(order);

            dto = await EnrichOrderAsync(dto);

            _logger.LogInformation("Returning OrderDto for OrderId {OrderId}: {@OrderDto}", request.OrderId, dto);

            return dto;
        }
        private async Task<OrderDto> EnrichOrderAsync(OrderDto orderDto)
        {
            try
            {
                var paymentTask = _paymentServiceRpcClient?.GetPaymentAsync(orderDto.Id);
                var shippingTask = _shippingServiceRpcClient.GetShippingMethodByIdAsync(orderDto.ShippingMethodId);

                if (paymentTask != null)
                    await Task.WhenAll(paymentTask, shippingTask);

                var payment = paymentTask != null ? await paymentTask : null;
                var shipping = await shippingTask;

                orderDto.ShippingMethod = shipping?.Name ?? "Unknown";
                orderDto.PaymentStatus = payment?.Status?.ToString() ?? PaymentStatus.Unknown.ToString();
                orderDto.PaymentId = payment?.PaymentId ?? Guid.Empty;

                _logger.LogInformation(
                    "Enriched OrderId {OrderId}: PaymentStatus={PaymentStatus}, ShippingMethod={ShippingMethod}",
                    orderDto.Id,
                    orderDto.PaymentStatus,
                    orderDto.ShippingMethod
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enrich OrderId {OrderId}. Setting defaults.", orderDto.Id);
                orderDto.ShippingMethod = "Unknown";
                orderDto.PaymentStatus = PaymentStatus.Unknown.ToString();
                orderDto.PaymentId = Guid.Empty;
            }

            return orderDto;
        }
    }
}
