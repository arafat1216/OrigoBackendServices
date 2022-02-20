using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class GlobalSubscriptionProductProfile : Profile
    {
        public GlobalSubscriptionProductProfile()
        {
            CreateMap<SubscriptionProduct, GlobalSubscriptionProductDTO>()
           .ForMember(destination => destination.Datapackages,
               opt => opt.MapFrom(src => src.DataPackages.Select(s => s.DataPackageName)))
           .ForMember(destination => destination.Name,
               opt => opt.MapFrom(src => src.SubscriptionName))
           .ForMember(destination => destination.OperatorId,
               opt => opt.MapFrom(src => src.Operator.Id));
        }
    }
}
