using AutoMapper;
using CartService.Application.Mapping;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.PaymentId));


            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<OrderItemPictureUrlResolver>());


        }
    }
}
