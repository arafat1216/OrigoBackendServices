using AutoMapper;
using SubscriptionManagement.API.ViewModels;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.Mappings
{
    public class OperatorProfile : Profile
    {
        public OperatorProfile()
        {
            CreateMap<Operator, OperatorDTO>();
            CreateMap<Operator, OperatorViewModel>();
        }
    }
}
