using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class CustomerStandardPrivateSubscriptionProductProfile : Profile
    {
        public CustomerStandardPrivateSubscriptionProductProfile()
        {
            CreateMap<CustomerStandardPrivateSubscriptionProduct, CustomerStandardPrivateSubscriptionProductDTO>();
        }
    }
}
