﻿using AutoMapper;
using System.Text.Json;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Mappings
{
    public class SubscriptionOrderProfile : Profile
    {
        public SubscriptionOrderProfile()
        {
            CreateMap<ISubscriptionOrder, SubscriptionOrderListItemDTO>();
        }
    }
}