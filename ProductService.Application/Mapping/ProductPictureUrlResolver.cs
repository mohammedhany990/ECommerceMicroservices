using AutoMapper;
using Microsoft.Extensions.Configuration;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;

namespace ProductService.Application.Mapping
{
    public class ProductPictureUrlResolver : IValueResolver<Product, ProductDto, string>
    {
        private readonly IConfiguration _configuration;

        public ProductPictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ImageUrl))
                return $"{_configuration["APIBaseUrl"]}/{source.ImageUrl}";

            return string.Empty;
        }
    }
}
