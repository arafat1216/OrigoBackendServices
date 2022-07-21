using AutoMapper;
using CustomerServices.Models;
using CustomerServices.ServiceModels;

namespace Customer.API.Mappings
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<Organization, OrganizationDTO>()
                .ForMember(d => d.PartnerId, opts => opts.MapFrom(o => o.Partner.ExternalId))
                .ForMember(d => d.Location, opts => opts.MapFrom(o => o.PrimaryLocation))
                .ForMember(d => d.Status, opts => opts.MapFrom(o => o.CustomerStatus))
                .ForMember(d => d.StatusName, opts => opts.MapFrom(o => o.CustomerStatus.ToString()));

        }
    }
    }

