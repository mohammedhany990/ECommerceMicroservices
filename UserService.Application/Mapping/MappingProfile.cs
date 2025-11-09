using AutoMapper;
using UserService.Application.Commands.RegisterUser;
using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterUserCommand, User>()
               .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
               .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<User, UserDto>();
        }
    }
}
