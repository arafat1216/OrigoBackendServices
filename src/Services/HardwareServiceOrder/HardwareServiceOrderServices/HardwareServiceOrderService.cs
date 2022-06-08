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

        /// <inheritdoc cref="IHardwareServiceOrderService.ConfigureLoanPhoneAsync(Guid, string, string, Guid)"/>
        public async Task<CustomerSettingsDTO> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmail, Guid callerId)
        {
            var entity = await _hardwareServiceOrderRepository.ConfigureLoanPhoneAsync(customerId, loanPhoneNumber, loanPhoneEmail, callerId);

            var dto = _mapper.Map<CustomerSettingsDTO>(entity);

            dto.ApiUserName = await _hardwareServiceOrderRepository.GetServiceIdAsync(customerId);

            return dto;
        }

        //TODO: It should be split up so the CustomerSettings and CustomerServiceProvider settings are not done together
        /// <inheritdoc cref="IHardwareServiceOrderService.ConfigureServiceIdAsync(Guid, CustomerSettingsDTO, Guid)"/>
        public async Task<CustomerSettingsDTO> ConfigureServiceIdAsync(Guid customerId, CustomerSettingsDTO customerSettings, Guid callerId)
        {
            var entity = await _hardwareServiceOrderRepository.ConfigureServiceIdAsync(customerId, customerSettings.AssetCategoryIds, customerSettings.ProviderId, customerSettings.LoanDevicePhoneNumber, customerSettings.LoanDeviceEmail, callerId, customerSettings.ApiUserName, customerSettings.ApiPassword);

            var dto = _mapper.Map<CustomerSettingsDTO>(entity);

            dto.ApiUserName = customerSettings.ApiUserName;

            return dto;
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

            var dto = _mapper.Map<CustomerSettingsDTO>(entity) ?? new CustomerSettingsDTO { CustomerId = customerId };

            dto.ApiUserName = await _hardwareServiceOrderRepository.GetServiceIdAsync(customerId);
            return dto;
        }

        public Task<HardwareServiceOrderDTO> UpdateHardwareServiceOrderAsync(Guid customerId, Guid orderId, HardwareServiceOrderDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
