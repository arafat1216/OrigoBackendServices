using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class PrivateSubscriptionProfile : Profile
    {
        public PrivateSubscriptionProfile()
        {
            CreateMap<PrivateSubscriptionDTO, PrivateSubscription>();
            CreateMap<PrivateSubscription, PrivateSubscriptionDTO>();
        }
    }
}
