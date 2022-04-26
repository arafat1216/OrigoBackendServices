﻿using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices
{
    public class HardwareServiceOrderService : IHardwareServiceOrderService
    {
        private readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;
        private readonly IMapper _mapper;
        public HardwareServiceOrderService(IHardwareServiceOrderRepository hardwareServiceOrderRepository, IMapper mapper)
        {
            _hardwareServiceOrderRepository = hardwareServiceOrderRepository;
            _mapper = mapper;
        }

        public async Task<CustomerSettingsDTO> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmain, Guid callerId)
        {
            var entity = await _hardwareServiceOrderRepository.ConfigureLoanPhoneAsync(customerId, loanPhoneNumber, loanPhoneEmain, callerId);
            return _mapper.Map<CustomerSettingsDTO>(entity);
        }

        public async Task<CustomerSettingsDTO> ConfigureServiceIdAsync(Guid customerId, string serviceId, Guid callerId)
        {
            var entity = await _hardwareServiceOrderRepository.ConfigureServiceIdAsync(customerId, serviceId, callerId);
            return _mapper.Map<CustomerSettingsDTO>(entity);
        }

        public async Task<CustomerSettingsDTO> GetSettingsAsync(Guid customerId)
        {
            var entity = await _hardwareServiceOrderRepository.GetSettingsAsync(customerId);
            return _mapper.Map<CustomerSettingsDTO>(entity);
        }
    }
}