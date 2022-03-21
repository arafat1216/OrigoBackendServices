using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings;

public class CustomerSubscriptionProductProfile : Profile
{
    public CustomerSubscriptionProductProfile()
    {
        CreateMap<CustomerSubscriptionProduct, CustomerSubscriptionProductDTO>()
            .ForMember(destination => destination.DataPackages,
                opt => opt.MapFrom(src => src.DataPackages.Select(s => s.DataPackageName)))
            .ForMember(destination => destination.IsGlobal, 
                opt => opt.MapFrom(src => src.GlobalSubscriptionProduct != null))
            .ForMember(destination => destination.Name, 
                opt => opt.MapFrom(src => src.SubscriptionName))
            .ForMember(destination => destination.OperatorId,
                opt => opt.MapFrom(src => src.Operator.Id));
    }
}