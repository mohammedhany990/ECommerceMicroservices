using AutoMapper;
using CategoryService.Application.DTOs;
using CategoryService.Domain.Data;

namespace CategoryService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>();
        }
    }
}
