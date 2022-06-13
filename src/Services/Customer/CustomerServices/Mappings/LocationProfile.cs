using AutoMapper;
using CustomerServices.Models;
using CustomerServices.ServiceModels;

namespace CustomerServices.Mappings
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            CreateMap<Location, LocationDTO>()
                .ForMember(destination => destination.Id, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(destination => destination.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(destination => destination.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(destination => destination.Address1, opt => opt.MapFrom(src => src.Address1))
                .ForMember(destination => destination.Address2, opt => opt.MapFrom(src => src.Address2))
                .ForMember(destination => destination.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
                .ForMember(destination => destination.City, opt => opt.MapFrom(src => src.City))
                .ForMember(destination => destination.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(destination => destination.RecipientType, opt => opt.MapFrom(src => src.RecipientType));

        }
    }
}
