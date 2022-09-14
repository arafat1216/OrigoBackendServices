using AutoMapper;
using OrigoApiGateway.Models.TechstepCore;

namespace OrigoApiGateway.Mappings
{
    public class TechstepCoreProfile : Profile
    {
        public TechstepCoreProfile()
        {
            CreateMap<TechstepCoreData, TechstepCustomer>()
            .ForMember(destination => destination.OrganizationNumber, opt => opt.MapFrom(src => src.OrgNumber))
                .ForMember(destination => destination.Country, opt => opt.MapFrom(src => src.CountryCode));
            CreateMap<IEnumerable<TechstepCoreData>, TechstepCustomers>().ForMember(dto => dto.Data, opt =>
            {
                opt.MapFrom(src => src.Where(a => !a.IsInactive).ToList());
            });
        }
    }
 
}
