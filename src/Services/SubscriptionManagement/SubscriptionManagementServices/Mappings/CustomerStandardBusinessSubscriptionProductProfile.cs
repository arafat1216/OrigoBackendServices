using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class CustomerStandardBusinessSubscriptionProductProfile : Profile
    {
        public CustomerStandardBusinessSubscriptionProductProfile()
        {
            CreateMap<CustomerStandardBusinessSubscriptionProduct, CustomerStandardBusinessSubscriptionProductDTO>();
        }
    }
}
