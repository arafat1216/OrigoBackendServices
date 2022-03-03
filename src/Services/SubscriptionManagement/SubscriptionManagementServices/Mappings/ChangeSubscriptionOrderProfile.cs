using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class ChangeSubscriptionOrderProfile : Profile
    {
        public ChangeSubscriptionOrderProfile()
        {
            CreateMap<ChangeSubscriptionOrder, ChangeSubscriptionOrderDTO>();
        }
    }
}
