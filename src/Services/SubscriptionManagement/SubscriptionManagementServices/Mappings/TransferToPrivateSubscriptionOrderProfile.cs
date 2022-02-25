using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    internal class TransferToPrivateSubscriptionOrderProfile : Profile
    {
        public TransferToPrivateSubscriptionOrderProfile()
        {
            CreateMap<TransferToPrivateSubscriptionOrderDTO, TransferToPrivateSubscriptionOrder>()
                .ForMember(d => d.UserInfo, opts => opts.MapFrom(s => s.UserInfo))
                .ForMember(d => d.OperatorName, opts => opts.MapFrom(s => s.OperatorName))
                .ForMember(d => d.NewSubscription, opts => opts.MapFrom(s => s.NewSubscription))
                .ForMember(d => d.OrderExecutionDate, opts => opts.MapFrom(s => s.OrderExecutionDate));

            CreateMap<TransferToPrivateSubscriptionOrder, TransferToPrivateSubscriptionOrderDTO>();
        }
    }
}
