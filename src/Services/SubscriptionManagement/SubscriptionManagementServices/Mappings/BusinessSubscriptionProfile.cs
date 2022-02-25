using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class BusinessSubscriptionProfile : Profile
    {
        public BusinessSubscriptionProfile()
        {
            CreateMap<BusinessSubscriptionDTO, BusinessSubscription>();
            CreateMap<BusinessSubscription, BusinessSubscriptionDTO>();
        }
    }
}
