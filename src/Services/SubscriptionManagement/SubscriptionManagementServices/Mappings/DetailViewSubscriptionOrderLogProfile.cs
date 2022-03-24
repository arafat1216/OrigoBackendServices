﻿using AutoMapper;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class DetailViewSubscriptionOrderLogProfile : Profile
    {
        public DetailViewSubscriptionOrderLogProfile()
        {
            CreateMap<OrderSimSubscriptionOrderDTO, DetailViewSubscriptionOrderLog>()
                .ForMember(dto => dto.OrderSim, opts =>
                    opts.MapFrom(m => new OrderSimSubscriptionOrderDTO(m)));

            CreateMap<ActivateSimOrderDTOResponse, DetailViewSubscriptionOrderLog>()
                .ForMember(dto => dto.ActivateSim, opts =>
                    opts.MapFrom(m => new ActivateSimOrderDTOResponse(m)));

            CreateMap<TransferToBusinessSubscriptionOrderDTOResponse, DetailViewSubscriptionOrderLog>()
               .ForMember(dto => dto.TransferToBusiness, opts =>
                   opts.MapFrom(m => new TransferToBusinessSubscriptionOrderDTOResponse(m)));

            CreateMap<TransferToPrivateSubscriptionOrderDTOResponse, DetailViewSubscriptionOrderLog>()
                 .ForMember(dto => dto.TransferToPrivateSub,
                 opts => opts.MapFrom(m => new TransferToPrivateSubscriptionOrderDTOResponse(m))); 

            CreateMap<ChangeSubscriptionOrderDTO, DetailViewSubscriptionOrderLog>()
             .ForMember(dto => dto.ChangeSubscription, opts =>
                 opts.MapFrom(m => new ChangeSubscriptionOrderDTO(m)));

            CreateMap<CancelSubscriptionOrderDTOResponse, DetailViewSubscriptionOrderLog>()
            .ForMember(dto => dto.CancelSubscription, opts =>
                opts.MapFrom(m => new CancelSubscriptionOrderDTOResponse(m)));

            CreateMap<NewSubscriptionOrderDTO, DetailViewSubscriptionOrderLog>()
            .ForMember(dto => dto.NewSubscription, opts =>
                opts.MapFrom(m => new NewSubscriptionOrderDTO(m)));


        }
    }
}
