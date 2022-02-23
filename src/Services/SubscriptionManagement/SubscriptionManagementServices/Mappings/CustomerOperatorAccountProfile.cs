using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class CustomerOperatorAccountProfile : Profile
    {
        public CustomerOperatorAccountProfile()
        {
            CreateMap<CustomerOperatorAccount, CustomerOperatorAccountDTO>();
        }
    }
}
