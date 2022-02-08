using AutoMapper;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.Mappings
{
    public class OperatorProfile : Profile
    {
        public OperatorProfile()
        {
            CreateMap<SubscriptionManagementServices.Models.Operator, OperatorDTO>();
            CreateMap<SubscriptionManagementServices.Models.Operator, ViewModels.Operator >();
        }
    }
}
