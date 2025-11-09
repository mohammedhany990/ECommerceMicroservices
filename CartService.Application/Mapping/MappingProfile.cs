using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Mapping
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {

            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.TotalPrice,
                    opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity * i.UnitPrice)));


            CreateMap<CartItem, CartItemDto>().ReverseMap();
        }

    }
}
