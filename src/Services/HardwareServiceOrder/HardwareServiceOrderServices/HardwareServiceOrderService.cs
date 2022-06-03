using AutoMapper;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using HardwareServiceOrderServices.Services;

namespace HardwareServiceOrderServices
{
    public class HardwareServiceOrderService : IHardwareServiceOrderService
    {
        private readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;
        private readonly IMapper _mapper;
        private IEmailService _emailService;
        private readonly IProviderFactory _providerFactory;
        private readonly Dictionary<ServiceStatusEnum, ServiceOrderStatusHandlerService> _serviceOrderStatusHandlers;
        public HardwareServiceOrderService(
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IMapper mapper,
            IProviderFactory providerFactory,
            Dictionary<ServiceStatusEnum, ServiceOrderStatusHandlerService> serviceOrderStatusHandlers,
            IEmailService emailService)
        {
            _hardwareServiceOrderRepository = hardwareServiceOrderRepository;
            _mapper = mapper;
            _emailService = emailService;
            _providerFactory = providerFactory;
            _serviceOrderStatusHandlers = serviceOrderStatusHandlers;
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

        public async Task<HardwareServiceOrderDTO> CreateHardwareServiceOrderAsync(Guid customerId, HardwareServiceOrderDTO serviceOrderDTO)
        {
            var customerSettingDto = GetSettingsAsync(customerId).Result;

            var newExternalOrder = new NewExternalRepairOrderDTO(new Guid(), serviceOrderDTO.FirstName, serviceOrderDTO.LastName, serviceOrderDTO.PhoneNumber,
                    serviceOrderDTO.Email, serviceOrderDTO.OrganizationId, serviceOrderDTO.OrganizationNumber, serviceOrderDTO.DeliveryAddress,
                    serviceOrderDTO.AssetInfo, serviceOrderDTO.ErrorDescription);

            //Creating Conmodo order
            var externalOrderResponseDTO =
                _repairProvider.CreateRepairOrderAsync(newExternalOrder,(int)ServiceTypeEnum.SUR, "23767").Result;

            var deliveryAddress = new DeliveryAddress(serviceOrderDTO.DeliveryAddress.Recipient, "", serviceOrderDTO.DeliveryAddress.Address1,
                serviceOrderDTO.DeliveryAddress.Address2, serviceOrderDTO.DeliveryAddress.PostalCode, serviceOrderDTO.DeliveryAddress.City,
                serviceOrderDTO.DeliveryAddress.Country);

            var fullName = serviceOrderDTO.FirstName + " " + serviceOrderDTO.LastName;
            var user = new User(new Guid(),fullName,serviceOrderDTO.Email);

            var serviceOrder = new HardwareServiceOrder(customerId, user, serviceOrderDTO.AssetInfo.AssetLifecycleId
                , serviceOrderDTO.AssetInfo.AssetName, deliveryAddress, serviceOrderDTO.UserDescription, serviceOrderDTO.ServiceProvider, "", "", serviceOrderDTO.ExternalProviderLink, serviceOrderDTO.ServiceType, serviceOrderDTO.ServiceStatus
               );

            //Creating order at Origo
            var a = await _hardwareServiceOrderRepository.CreateHardwareServiceOrder(serviceOrder);


            //Sending Confirmation Email

            if(a != null)
            {
                try
                {
                    
                    var orderConfirmationMail = new OrderConfirmationEmail
                    {
                        AssetId = serviceOrderDTO.AssetId.ToString(),
                        AssetName = serviceOrderDTO.AssetInfo.AssetName,
                        FirstName = serviceOrderDTO.FirstName,
                        OrderDate = a.CreatedDate.UtcDateTime,
                        OrderLink = serviceOrderDTO.ExternalProviderLink,
                        Recipient = serviceOrderDTO.DeliveryAddress.Recipient,
                        FaultCategory = serviceOrderDTO.FaultType,
                        LoanDeviceContact = customerSettingDto.LoanDevicePhoneNumber,
                        Subject = serviceOrderDTO.Email,
                        RepairType = serviceOrderDTO.FaultType
                    };
                    var emailResponse = _emailService.SendOrderConfirmationEmailAsync(orderConfirmationMail, "en");
                }
                catch (Exception ex)
                {
                    throw;
                }

            }

            //var orderDTO = _mapper.Map<Task<HardwareServiceOrderDTO>>(serviceOrderDTO);
            return serviceOrderDTO;
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

        /// <inheritdoc cref="IHardwareServiceOrderService.UpdateOrderStatusAsync"/>
        public async Task UpdateOrderStatusAsync()
        {
            var customerProviders = await _hardwareServiceOrderRepository.GetAllCustomerProvidersAsync();

            foreach (var customerProvider in customerProviders)
            {
                var provider = await _providerFactory.GetRepairProviderAsync(customerProvider.ServiceProviderId, customerProvider.ApiUserName, customerProvider.ApiPassword);

                var updateStarted = DateTime.UtcNow;

                var updatedOrders = await provider.GetUpdatedRepairOrdersAsync(customerProvider.LastUpdateFetched);

                foreach (var order in updatedOrders)
                {
                    if (!order.ExternalServiceEvents.Any())
                        continue;

                    var lastOrderStatus = order.ExternalServiceEvents.OrderByDescending(m => m.Timestamp).FirstOrDefault()?.ServiceStatusId;

                    var origoOrder = await _hardwareServiceOrderRepository.GetOrderByServiceProviderOrderIdAsync(order.ServiceProviderOrderId1);

                    if (origoOrder == null || origoOrder.StatusId == lastOrderStatus)
                        continue;

                    var lastOrderStatusEnum = (ServiceStatusEnum)lastOrderStatus;

                    var statusHandler = _serviceOrderStatusHandlers.ContainsKey(lastOrderStatusEnum) ? _serviceOrderStatusHandlers[lastOrderStatusEnum] : _serviceOrderStatusHandlers[ServiceStatusEnum.Unknown];

                    await statusHandler.UpdateServiceOrderStatusAsync(origoOrder.ExternalId, lastOrderStatusEnum);

                    //Add events in the log
                    var serviceEvents = _mapper.Map<IEnumerable<ServiceEvent>>(order.ExternalServiceEvents);

                    await _hardwareServiceOrderRepository.UpdateServiceEventsAsync(origoOrder, serviceEvents);
                }

                await _hardwareServiceOrderRepository.UpdateCustomerProviderLastUpdateFetchedAsync(customerProvider, updateStarted);
            }
        }

        public Task<HardwareServiceOrderDTO> UpdateHardwareServiceOrderAsync(Guid customerId, Guid orderId, HardwareServiceOrderDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
