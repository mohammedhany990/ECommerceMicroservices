using AutoMapper;

using Microsoft.Extensions.Configuration;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace CartService.Application.Mapping
{
    public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly IConfiguration _configuration;

        public OrderItemPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        string IValueResolver<OrderItem, OrderItemDto, string>.Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ImageUrl))
                return $"{_configuration["APIBaseUrl"]}/{source.ImageUrl}";
            return string.Empty;
        }
    }
}
