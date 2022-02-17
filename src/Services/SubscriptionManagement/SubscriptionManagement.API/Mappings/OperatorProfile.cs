using AutoMapper;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.Mappings
{
    public class OperatorProfile : Profile
    {
        public OperatorProfile()
        {
            CreateMap<SubscriptionManagementServices.Models.Operator, OperatorDTO>()
                .ForMember(destination => destination.Name, opt => opt.MapFrom(src => src.OperatorName));
            
            CreateMap<SubscriptionManagementServices.Models.Operator, ViewModels.Operator >()
            .ForMember(destination => destination.Name, opt => opt.MapFrom(src => src.OperatorName));
        }
    }
}
