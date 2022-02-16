using AutoMapper;
using SubscriptionManagement.API.ViewModels;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.Mappings;

public class CustomerSubscriptionProductProfile : Profile
{
    public CustomerSubscriptionProductProfile()
    {
        CreateMap<CustomerSubscriptionProductDTO, CustomerSubscriptionProduct>();
    }
}