using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;
using Shared.Enums;
using Shared.Messaging;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IMapper _mapper;
        private readonly CartServiceRpcClient _cartServiceRpcClient;
        private readonly IRabbitMqPublisher<CreateNotificationEvent> _publisher;
        private readonly ProductServiceRpcClient _productServiceRpcClient;
        private readonly ShippingServiceRpcClient _shippingServiceRpcClient;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        public CreateOrderCommandHandler(
            IRepository<Order> orderRepo,
            IMapper mapper,
           CartServiceRpcClient cartServiceRpcClient,
            IRabbitMqPublisher<CreateNotificationEvent> publisher,
            ProductServiceRpcClient productServiceRpcClient,
            ShippingServiceRpcClient shippingServiceRpcClient,
             ILogger<CreateOrderCommandHandler> logger)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _cartServiceRpcClient = cartServiceRpcClient;
            _publisher = publisher;
            _productServiceRpcClient = productServiceRpcClient;
            _shippingServiceRpcClient = shippingServiceRpcClient;
            _logger = logger;
        }
        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating order for UserId: {UserId}", request.UserId);

            var cart = await _cartServiceRpcClient.GetCartForUser(request.UserId);
            if (cart is null || !cart.Items.Any())
            {
                _logger.LogWarning("Cart is empty for UserId: {UserId}", request.UserId);
                throw new Exception("Cart is empty. Cannot create order.");
            }

            var orderItems = new List<OrderItem>();


            foreach (var item in cart.Items)
            {
                var product = await _productServiceRpcClient.GetProductByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found.");

                var stockReserved = await _productServiceRpcClient.ReserveStockAsync(product.Id, item.Quantity);
                if (!stockReserved)
                    throw new Exception($"Not enough stock for product {product.Name}. Requested: {item.Quantity}");

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


            var shippingResult = await _shippingServiceRpcClient.CalculateShippingCostAsync(
                    new Shared.DTOs.ShippingCostRequestDto(request.ShippingAddressId, request.ShippingMethodId));

           
            if (shippingResult == null)
            {
                _logger.LogWarning("ShippingService returned null for UserId {UserId}", request.UserId);
                throw new Exception("Unable to calculate shipping cost.");
            }


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

            _logger.LogInformation("Order created successfully. OrderId: {OrderId}, UserId: {UserId}", order.Id, request.UserId);


            return orderDto;

        }
    }
}
