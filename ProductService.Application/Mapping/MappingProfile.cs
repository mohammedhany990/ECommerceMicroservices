using AutoMapper;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.Commands.UpdateProduct;
using ProductService.Application.DTOs;
using ProductService.Domain.Entities;

namespace ProductService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            CreateMap<CreateProductCommand, Product>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
               .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
               .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
               .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
               .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<UpdateProductCommand, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                //.ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null || srcMember is decimal || srcMember is int));
            ;


            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<ProductPictureUrlResolver>())
                ;

        }
    }
}
