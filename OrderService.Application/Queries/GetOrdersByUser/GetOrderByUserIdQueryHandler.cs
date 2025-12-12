using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;

namespace OrderService.Application.Queries.GetOrdersByUser
{
    public class GetOrderByUserIdQueryHandler : IRequestHandler<GetOrderByUserIdQuery, IReadOnlyList<OrderDto>>
    {
        private readonly IRepository<Order> _repository;
        private readonly IMapper _mapper;
        private readonly ShippingServiceRpcClient _shippingServiceRpcClient;
        private readonly PaymentServiceRpcClient _paymentServiceRpcClient;
        private readonly ILogger<GetOrderByUserIdQueryHandler> _logger;

        public GetOrderByUserIdQueryHandler(
            IRepository<Order> repository,
            IMapper mapper,
            ShippingServiceRpcClient shippingServiceRpcClient,
            PaymentServiceRpcClient paymentServiceRpcClient,
            ILogger<GetOrderByUserIdQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _shippingServiceRpcClient = shippingServiceRpcClient ?? throw new ArgumentNullException(nameof(shippingServiceRpcClient));
            _paymentServiceRpcClient = paymentServiceRpcClient ?? throw new ArgumentNullException(nameof(paymentServiceRpcClient));
            _logger = logger;
        }

        public async Task<IReadOnlyList<OrderDto>> Handle(GetOrderByUserIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError("Request object is null in GetOrdersByUser");
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation("Starting GetOrdersByUser for UserId {UserId}", request.UserId);


            var userOrders = await _repository.GetAllAsync(
                x => x.UserId == request.UserId,
                include: q => q.Include(o => o.Items)
            );

            if (userOrders == null || !userOrders.Any())
            {
                _logger.LogWarning("No orders found for UserId {UserId}", request.UserId);
                return Array.Empty<OrderDto>();
            }

            _logger.LogInformation("{Count} orders found for UserId {UserId}", userOrders.Count(), request.UserId);

            var orderDtos = _mapper.Map<IReadOnlyList<OrderDto>>(userOrders);

            var enrichmentTasks = orderDtos.Select(async orderDto =>
            {
                _logger.LogInformation("Enriching OrderId {OrderId} for UserId {UserId}", orderDto.Id, request.UserId);

                try
                {
                    var payment = await _paymentServiceRpcClient.GetPaymentStatusAsync(orderDto.Id);
                    var shippingMethod = await _shippingServiceRpcClient.GetShippingMethodByIdAsync(orderDto.ShippingMethodId);

                    orderDto.ShippingMethod = shippingMethod?.Name ?? "Unknown";
                    
                    orderDto.PaymentId = payment?.PaymentId ?? Guid.Empty;
                    orderDto.PaymentStatus = payment?.Status ?? "Unknown";


                    _logger.LogInformation(
                        "Enrich completed for OrderId {OrderId}: PaymentStatus={PaymentStatus}, ShippingMethod={ShippingMethod}",
                        orderDto.Id,
                        orderDto.PaymentStatus,
                        orderDto.ShippingMethod
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error enriching OrderId {OrderId} for UserId {UserId}",
                        orderDto.Id,
                        request.UserId);

                    orderDto.PaymentStatus = "Error";
                    orderDto.ShippingMethod = "Error";
                }
            });

            await Task.WhenAll(enrichmentTasks);

            _logger.LogInformation("Completed GetOrdersByUser for UserId {UserId}", request.UserId);

            return orderDtos;
        }
    }

}
