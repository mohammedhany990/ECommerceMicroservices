using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace CartService.Application.Mapping
{
    public class CartItemPictureUrlResolver : IValueResolver<CartItem, CartItemDto, string>
    {
        private readonly IConfiguration _configuration;

        public CartItemPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        string IValueResolver<CartItem, CartItemDto, string>.Resolve(CartItem source, CartItemDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ImageUrl))
                return $"{_configuration["APIBaseUrl"]}/{source.ImageUrl}";
            return string.Empty;
        }
    }
}
