using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using AutoMapper;

namespace ApartmentRentals.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SpaceCreateDto, Space>();
            CreateMap<SpaceNoIdDto, Space>();
            CreateMap<Space, SpaceListDto>();
            CreateMap<LandlordNoIdDto, Landlord>();
            CreateMap<RentalContractNoIdDto, RentalContract>();
            CreateMap<TenantNoIdDto, Tenant>();
            CreateMap<UserNoIdDto, User>();
        }
    }
}
