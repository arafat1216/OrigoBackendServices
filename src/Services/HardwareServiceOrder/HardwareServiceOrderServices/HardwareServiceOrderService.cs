using AutoMapper;
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

        public async Task<CustomerSettingsDto> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmain, Guid callerId)
        {
            var entity = await _hardwareServiceOrderRepository.ConfigureLoanPhoneAsync(customerId, loanPhoneNumber, loanPhoneEmain, callerId);
            return _mapper.Map<CustomerSettingsDto>(entity);
        }

        public async Task<CustomerSettingsDto> ConfigureServiceIdAsync(Guid customerId, string serviceId, Guid callerId)
        {
            var entity = await _hardwareServiceOrderRepository.ConfigureServiceIdAsync(customerId, serviceId, callerId);
            return _mapper.Map<CustomerSettingsDto>(entity);
        }

        public Task<HardwareServiceOrderDTO> CreateHardwareServiceOrderAsync(Guid customerId, HardwareServiceOrderDTO model)
        {
            throw new NotImplementedException();
        }

        public Task<HardwareServiceOrderDTO> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task<List<HardwareServiceOrderLogDTO>> GetHardwareServiceOrderLogsAsync(Guid customerId, Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task<List<HardwareServiceOrderDTO>> GetHardwareServiceOrdersAsync(Guid customerId)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomerSettingsDTO> GetSettingsAsync(Guid customerId)
        {
            var entity = await _hardwareServiceOrderRepository.GetSettingsAsync(customerId);
            return _mapper.Map<CustomerSettingsDto>(entity);
        }

        public Task<HardwareServiceOrderDTO> UpdateHardwareServiceOrderAsync(Guid customerId, Guid orderId, HardwareServiceOrderDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
