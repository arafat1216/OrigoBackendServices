using AutoMapper;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.Mappings
{
    public class SubscriptionProductProfile : Profile
    {
        public SubscriptionProductProfile()
        {
            CreateMap<ViewModels.SubscriptionProduct, CustomerSubscriptionProductDTO>();
            CreateMap<CustomerSubscriptionProductDTO, ViewModels.SubscriptionProduct>();
        }
    }
}
