using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Interfaces;
using OrderService.Infrastructure.MessageBus;
using OrderService.Infrastructure.Services;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IMapper _mapper;
        private readonly CartServiceRpcClient _cartServiceRpcClient;
        private readonly ProductServiceClient _productServiceClient;
        private readonly ShippingServiceClient _shippingServiceClient;
        private readonly IRabbitMqPublisher<CreateNotificationEvent> _publisher;

        public CreateOrderCommandHandler(
            IRepository<Order> orderRepo,
            IMapper mapper,
           CartServiceRpcClient cartServiceRpcClient,
            ProductServiceClient productServiceClient,
            ShippingServiceClient shippingServiceClient,
            IRabbitMqPublisher<CreateNotificationEvent> publisher)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _cartServiceRpcClient = cartServiceRpcClient;
            _productServiceClient = productServiceClient;
            _shippingServiceClient = shippingServiceClient;
            _publisher = publisher;
        }
        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {

            var cart = await _cartServiceRpcClient.GetCartForUser(request.UserId);
            if (cart is null || !cart.Items.Any())
                throw new Exception("Cart is empty. Cannot create order.");

            var orderItems = new List<OrderItem>();


            foreach (var item in cart.Items)
            {
                var product = await _productServiceClient.GetProductByIdAsync(item.ProductId);
                if (product is null)
                    throw new Exception($"Product with ID {item.ProductId} not found.");

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity,
                    ImageUrl = product.ImageUrl
                });
            }
            var subtotal = orderItems.Sum(item => item.UnitPrice * item.Quantity);


            var shippingResult = await _shippingServiceClient.CalculateShippingCostAsync(
                new Shared.DTOs.ShippingCostRequestDto(request.ShippingAddressId, request.ShippingMethodId), request.AuthToken);

            if (shippingResult == null)
                throw new Exception("Unable to calculate shipping cost.");

            var order = new Order
            {
                UserId = request.UserId,
                ShippingAddressId = request.ShippingAddressId,
                ShippingMethodId = shippingResult.ShippingMethodId,
                Subtotal = subtotal,
                ShippingCost = shippingResult.Cost,
                TotalAmount = subtotal + shippingResult.Cost,
                Status = OrderStatus.Pending,
                Items = orderItems,
                CreatedAt = DateTime.UtcNow,
                ExpectedDeliveryDate = DateTime.UtcNow.AddDays(shippingResult.EstimatedDeliveryDays),

            };

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            var orderDto = _mapper.Map<OrderDto>(order);
            orderDto.ShippingMethod = shippingResult.MethodName;


            var notifEvent = new CreateNotificationEvent
            {
                UserId = request.UserId,
                To = request.Email,
                Subject = "Your Order Has Been Created",
                Body = $"Your order #{order.Id} has been successfully created."
            };

            _publisher.Publish(notifEvent);


            return orderDto;

        }
    }
}
