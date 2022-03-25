using AutoMapper;
using OrigoApiGateway.Models.SubscriptionManagement.Backend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;

namespace OrigoApiGateway.Mappings.SubscriptionManagement
{
    public class CancelSubscriptionProfile : Profile
    {
        public CancelSubscriptionProfile()
        {
            CreateMap<CancelSubscriptionOrder, CancelSubscriptionOrderDTO>();
        }
    }
}
