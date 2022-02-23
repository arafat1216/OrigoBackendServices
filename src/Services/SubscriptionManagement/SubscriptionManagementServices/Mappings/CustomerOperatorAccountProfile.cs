using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class CustomerOperatorAccountProfile : Profile
    {
        public CustomerOperatorAccountProfile()
        {
            CreateMap<CustomerOperatorAccount,CustomerOperatorAccountDTO>()
                .ForMember(destination => destination.AccountName,
                opt => opt.MapFrom(src => src.AccountName))
            .ForMember(destination => destination.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(destination => destination.OperatorId,
                opt => opt.MapFrom(src => src.OperatorId))
            .ForMember(destination => destination.AccountNumber,
                opt => opt.MapFrom(src => src.AccountNumber));  
        }
    }
}
