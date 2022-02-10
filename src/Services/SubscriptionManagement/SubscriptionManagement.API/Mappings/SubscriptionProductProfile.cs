using AutoMapper;
using SubscriptionManagement.API.ViewModels;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.Mappings
{
    public class SubscriptionProductProfile : Profile
    {
        public SubscriptionProductProfile()
        {
            CreateMap<SubscriptionManagementServices.Models.SubscriptionProduct, ViewModels.SubscriptionProduct>()
                .ForMember(destination => destination.Datapackages, opt => opt.MapFrom(src => src.DataPackages.Select(s=>s.DatapackageName)));

        }
    }
}
