﻿using AutoMapper;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings;

public class CustomerSubscriptionProductProfile : Profile
{
    public CustomerSubscriptionProductProfile()
    {
        CreateMap<CustomerSubscriptionProduct, CustomerSubscriptionProductDTO>()
            .ForMember(destination => destination.Datapackages,
                opt => opt.MapFrom(src => src.DataPackages.Select(s => s.DataPackageName)))
            .ForMember(destination => destination.isGlobal, 
                opt => opt.MapFrom(src => src.GlobalSubscriptionProduct != null));
    }
}