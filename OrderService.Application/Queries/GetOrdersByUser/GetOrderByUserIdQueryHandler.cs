using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

        public GetOrderByUserIdQueryHandler(
            IRepository<Order> repository,
            IMapper mapper,
            ShippingServiceRpcClient shippingServiceRpcClient,
            PaymentServiceRpcClient paymentServiceRpcClient)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _shippingServiceRpcClient = shippingServiceRpcClient ?? throw new ArgumentNullException(nameof(shippingServiceRpcClient));
            _paymentServiceRpcClient = paymentServiceRpcClient ?? throw new ArgumentNullException(nameof(paymentServiceRpcClient));
        }

        public async Task<IReadOnlyList<OrderDto>> Handle(GetOrderByUserIdQuery request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var userOrders = await _repository.GetAllAsync(
                x => x.UserId == request.UserId,
                include: q => q.Include(o => o.Items)
            );

            if (userOrders == null || !userOrders.Any())
                return Array.Empty<OrderDto>();

            var orderDtos = _mapper.Map<IReadOnlyList<OrderDto>>(userOrders);

            var enrichmentTasks = orderDtos.Select(async orderDto =>
            {
                try
                {
                    var paymentTask = _paymentServiceRpcClient.GetPaymentStatusAsync(orderDto.Id);
                    var shippingTask = _shippingServiceRpcClient.GetShippingMethodByIdAsync(orderDto.ShippingMethodId);

                    await Task.WhenAll(paymentTask, shippingTask);

                    var payment = paymentTask.Result;
                    orderDto.PaymentId = payment?.PaymentId ?? Guid.Empty;
                    orderDto.PaymentStatus = payment?.Status ?? "Unknown";

                    var shippingMethod = shippingTask.Result;
                    orderDto.ShippingMethod = shippingMethod?.Name ?? "Unknown";
                }
                catch (Exception ex)
                {
                    orderDto.PaymentStatus = "Error";
                    orderDto.ShippingMethod = "Error";
                }
            });

            await Task.WhenAll(enrichmentTasks);

            return orderDtos;
        }
    }

}
