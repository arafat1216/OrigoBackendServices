using AutoMapper;
using SubscriptionManagement.API.ViewModels;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.Mappings
{
    public class SubscriptionProductProfile : Profile
    {
        public SubscriptionProductProfile()
        {
            CreateMap<SubscriptionProduct, SubscriptionProductViewModel>();

        }
    }
}
