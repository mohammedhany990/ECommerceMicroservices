using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Services;

namespace OrderService.Application.Queries.GetOrdersByUser
{
    public class GetOrderByUserIdQueryHandler : IRequestHandler<GetOrderByUserIdQuery, IReadOnlyList<OrderDto>>
    {
        private readonly IRepository<Order> _repository;
        private readonly IMapper _mapper;
        private readonly ShippingServiceClient _shippingServiceClient;
        private readonly PaymentServiceClient _paymentServiceClient;

        public GetOrderByUserIdQueryHandler(IRepository<Order> repository,
            IMapper mapper,
            ShippingServiceClient shippingServiceClient,
            PaymentServiceClient paymentServiceClient)
        {
            _repository = repository;
            _mapper = mapper;
            _shippingServiceClient = shippingServiceClient;
            _paymentServiceClient = paymentServiceClient;
        }
        public async Task<IReadOnlyList<OrderDto>> Handle(GetOrderByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userOrders = await _repository.GetAllAsync(
                x => x.UserId == request.UserId,
                include: q => q.Include(i => i.Items)
            );

            if (!userOrders.Any())
                return new List<OrderDto>();

            var orderDtos = _mapper.Map<IReadOnlyList<OrderDto>>(userOrders);

            var tasks = orderDtos.Select(async orderDto =>
            {
                var payment = await _paymentServiceClient.GetPaymentStatusAsync(orderDto.Id, request.AuthToken);
                orderDto.PaymentId = payment?.PaymentId ?? Guid.Empty;
                orderDto.PaymentStatus = payment?.Status ?? "Unknown";

                var shippingMethod = await _shippingServiceClient.GetShippingMethodByIdAsync(orderDto.ShippingMethodId, request.AuthToken);
                orderDto.ShippingMethod = shippingMethod?.Name ?? "Unknown";
            }).ToList();

            await Task.WhenAll(tasks);

            return orderDtos;
        }

    }
}
