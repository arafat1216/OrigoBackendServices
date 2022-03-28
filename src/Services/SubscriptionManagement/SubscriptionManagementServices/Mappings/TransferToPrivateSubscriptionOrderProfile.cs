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
                .ForMember(m => m.UserInfo, opts => opts.MapFrom(dto => dto.PrivateSubscription))
                .ForMember(m => m.CreatedBy, opts => opts.MapFrom(dto => dto.CallerId));

            CreateMap<TransferToPrivateSubscriptionOrder, TransferToPrivateSubscriptionOrderDTO>()
                .ForMember(dto => dto.PrivateSubscription, opts => opts.MapFrom(m => m.UserInfo))
                .ForMember(dto => dto.NewSubscription, opts => opts.MapFrom(m => m.NewSubscription))
                .ForMember(m => m.CallerId, opts => opts.MapFrom(dto => dto.CreatedBy));

            CreateMap<TransferToPrivateSubscriptionOrder, TransferToPrivateSubscriptionOrderDTOResponse>()
                .ForMember(dto => dto.PrivateSubscription, opts => opts.MapFrom(m => m.UserInfo))
                .ForMember(dto => dto.NewSubscriptionName, opts => opts.MapFrom(m => m.NewSubscription));

        }
    }
}
