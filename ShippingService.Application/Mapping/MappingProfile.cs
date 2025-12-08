using AutoMapper;
using ShippingService.Application.Commands.Addresses.CreateShippingAddress;
using ShippingService.Application.Commands.Addresses.UpdateShippingAddress;
using ShippingService.Application.Commands.Methods.CreateShippingMethod;
using ShippingService.Application.Commands.Methods.UpdateShippingMethod;
using ShippingService.Application.Commands.Shipments.CreateShipment;
using ShippingService.Application.Commands.Shipments.UpdateShipment;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;

namespace ShippingService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ===========================
            // Shipping Methods
            // ===========================

            // Create Shipping Method
            CreateMap<CreateShippingMethodCommand, ShippingMethod>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Update Shipping Method
            CreateMap<UpdateShippingMethodCommand, ShippingMethod>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Entity → DTO
            CreateMap<ShippingMethod, ShippingMethodDto>();




            // ===========================
            // Shipping Addresses
            // ===========================

            // Create Shipping Address
            CreateMap<CreateShippingAddressCommand, ShippingAddress>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Update Shipping Address
            CreateMap<UpdateShippingAddressCommand, ShippingAddress>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Entity → DTO
            CreateMap<ShippingAddress, ShippingAddressDto>();



            // ===========================
            // Shipments
            // ===========================

            // Create Shipment
            CreateMap<CreateShipmentCommand, Shipment>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.ShippedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.TrackingNumber,
                           opt => opt.MapFrom(src => string.IsNullOrEmpty(src.TrackingNumber) ? "TBD" : src.TrackingNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Pending"));

            // Update Shipment
            CreateMap<UpdateShipmentCommand, Shipment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Entity → DTO
            CreateMap<Shipment, ShipmentDto>();
        }
    }
}
