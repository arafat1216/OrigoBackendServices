using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class CancelSubscriptionOrderProfile : Profile
    {
        public CancelSubscriptionOrderProfile()
        {
            CreateMap<CancelSubscriptionOrder, CancelSubscriptionOrderDTO>();
        }
    }
}
