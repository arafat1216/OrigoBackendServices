using AutoMapper;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.Mappings
{
    public class OperatorProfile : Profile
    {
        public OperatorProfile()
        {
            CreateMap<OperatorDTO, ViewModels.Operator >();
        }
    }
}
