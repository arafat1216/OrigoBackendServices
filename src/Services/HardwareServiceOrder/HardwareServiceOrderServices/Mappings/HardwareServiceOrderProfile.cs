﻿using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    public class HardwareServiceOrderProfile : Profile
    {
        public HardwareServiceOrderProfile()
        {
            CreateMap<HardwareServiceOrder, HardwareServiceOrderDTO>();
        }
    }
}
