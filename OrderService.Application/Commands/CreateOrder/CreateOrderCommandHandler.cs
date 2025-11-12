using AutoMapper;
using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Services;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IMapper _mapper;
        private readonly CartServiceClient _cartServiceClient;
        private readonly ProductServiceClient _productServiceClient;
        private readonly ShippingServiceClient _shippingServiceClient;

        public CreateOrderCommandHandler(IRepository<Order> orderRepo,
            IMapper mapper,
            CartServiceClient cartServiceClient,
            ProductServiceClient productServiceClient,
            ShippingServiceClient shippingServiceClient)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _cartServiceClient = cartServiceClient;
            _productServiceClient = productServiceClient;
            _shippingServiceClient = shippingServiceClient;
        }
        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {

            var cart = await _cartServiceClient.GetCartForUser(request.AuthToken);
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
                new ShippingCostRequestDto(request.ShippingAddressId, request.ShippingMethodId));

            if (shippingResult == null)
                throw new Exception("Unable to calculate shipping cost.");

            var order = new Order
            {
                UserId = request.UserId,
                ShippingAddressId = request.ShippingAddressId,
                ShippingMethod = shippingResult.MethodName,
                PaymentMethod = request.PaymentMethodId,
                Subtotal = subtotal,
                ShippingCost = shippingResult.Cost,
                TotalAmount = subtotal + shippingResult.Cost,
                Status = OrderStatus.Pending,
                Items = orderItems
            };

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            var orderDto = _mapper.Map<OrderDto>(order);
            return orderDto;

        }
    }
}
