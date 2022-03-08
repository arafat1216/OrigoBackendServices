using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class ActivateSimOrderProfile : Profile
    {
        public ActivateSimOrderProfile()
        {
            CreateMap<ActivateSimOrder, ActivateSimOrderDTO>();
        }
    }
}
