using AutoMapper;
using Common.Interfaces;
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

            return dto;
        }

        /// <inheritdoc cref="IHardwareServiceOrderService.ConfigureCustomerSettingsAsync(Guid, Guid)"/>
        public async Task<CustomerSettingsDTO> ConfigureCustomerSettingsAsync(Guid customerId, Guid callerId)
        {
            var entity = await _hardwareServiceOrderRepository.ConfigureCustomerSettingsAsync(customerId, callerId);

            var dto = _mapper.Map<CustomerSettingsDTO>(entity);

            return dto;
        }

        public async Task<HardwareServiceOrderResponseDTO> CreateHardwareServiceOrderAsync(Guid customerId, HardwareServiceOrderDTO serviceOrderDTO)
        {
            var customerSetting = await GetSettingsAsync(customerId);

            if (customerSetting == null)
                throw new ArgumentException("Customer settings does not exist");

            var newExternalOrder = new NewExternalRepairOrderDTO(
                                    serviceOrderDTO.OrderedBy.Id,
                                    serviceOrderDTO.OrderedBy.FirstName,
                                    serviceOrderDTO.OrderedBy.LastName,
                                    serviceOrderDTO.OrderedBy.PhoneNumber,
                                    serviceOrderDTO.OrderedBy.Email,
                                    serviceOrderDTO.OrderedBy.OrganizationId,
                                    serviceOrderDTO.OrderedBy.OrganizationName,
                                    serviceOrderDTO.OrderedBy.OrganizationNumber,
                                    serviceOrderDTO.OrderedBy.PartnerId,
                                    serviceOrderDTO.OrderedBy.PartnerName,
                                    serviceOrderDTO.OrderedBy.PartnerOrganizationNumber,
                                    serviceOrderDTO.DeliveryAddress,
                                    serviceOrderDTO.AssetInfo,
                                    serviceOrderDTO.ErrorDescription);

            var customerProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(customerId, (int)ServiceProviderEnum.ConmodoNo);

            if (customerProvider == null)
                throw new ArgumentException($"Service provider is not configured for customer {customerId}", nameof(customerId));

            //Creating Conmodo order
            try
            {
                var repairProvider = await _providerFactory.GetRepairProviderAsync(customerProvider.ServiceProviderId, customerProvider.ApiUserName, customerProvider.ApiPassword);

                var newOrderId = Guid.NewGuid();

                var externalOrderResponseDTO = await repairProvider.CreateRepairOrderAsync(newExternalOrder, (int)ServiceTypeEnum.SUR, $"{newOrderId}");

                var deliveryAddress = _mapper.Map<DeliveryAddress>(serviceOrderDTO.DeliveryAddress);

                var owner = new ContactDetails(serviceOrderDTO.OrderedBy.Id, serviceOrderDTO.OrderedBy.FirstName, serviceOrderDTO.OrderedBy.Email);
                var serviceType = await _hardwareServiceOrderRepository.GetServiceTypeAsync((int)ServiceTypeEnum.SUR) ?? new ServiceType { Id = (int)ServiceTypeEnum.SUR };
                var serviceStatus = await _hardwareServiceOrderRepository.GetServiceStatusAsync((int)ServiceStatusEnum.Registered);

                var serviceOrder = new HardwareServiceOrder(
                    customerId,
                    owner,
                    serviceOrderDTO.AssetInfo.AssetLifecycleId,
                    deliveryAddress,
                    serviceOrderDTO.ErrorDescription,
                    customerProvider.ServiceProvider,
                    externalOrderResponseDTO.ServiceProviderOrderId1,
                    externalOrderResponseDTO.ServiceProviderOrderId2,
                    externalOrderResponseDTO.ExternalServiceManagementLink,
                    serviceType,
                    serviceStatus
                   );

                //Creating order at Origo
                var origoOrder = await _hardwareServiceOrderRepository.CreateHardwareServiceOrder(serviceOrder);

                var orderConfirmationMail = new OrderConfirmationEmail
                {
                    AssetId = $"{serviceOrderDTO.AssetInfo.AssetLifecycleId}",
                    AssetName = serviceOrderDTO.AssetInfo.AssetName,
                    FirstName = serviceOrderDTO.OrderedBy.FirstName,
                    OrderDate = origoOrder.DateCreated.UtcDateTime,
                    OrderLink = externalOrderResponseDTO.ExternalServiceManagementLink,
                    Recipient = serviceOrderDTO.OrderedBy.Email,
                    LoanDeviceContact = customerSetting.LoanDevicePhoneNumber,
                    Subject = OrderConfirmationEmail.SubjectKeyName,
                    PackageSlipLink = externalOrderResponseDTO.ExternalServiceManagementLink
                };

                await _emailService.SendOrderConfirmationEmailAsync(orderConfirmationMail, "en");

                var responseDto = new HardwareServiceOrderResponseDTO
                {
                    Created = origoOrder.DateCreated,
                    Updated = origoOrder.DateUpdated,
                    Id = origoOrder.ExternalId,
                    Events = new List<ExternalServiceEventDTO> { },
                    Owner = origoOrder.Owner.UserId,
                    ServiceProvider = (ServiceProviderEnum)origoOrder.ServiceProviderId,
                    Status = (ServiceStatusEnum)origoOrder.StatusId,
                    Type = (ServiceTypeEnum)origoOrder.ServiceTypeId,
                    AssetLifecycleId = origoOrder.AssetLifecycleId
                };

                return responseDto;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <inheritdoc cref="IHardwareServiceOrderService.GetHardwareServiceOrderAsync(Guid, Guid)"/>
        public async Task<HardwareServiceOrderResponseDTO> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId)
        {
            var orderEntity = await _hardwareServiceOrderRepository.GetOrderAsync(orderId);

            return _mapper.Map<HardwareServiceOrderResponseDTO>(orderEntity);
        }

        public Task<List<HardwareServiceOrderLogDTO>> GetHardwareServiceOrderLogsAsync(Guid customerId, Guid orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedModel<HardwareServiceOrderResponseDTO>> GetHardwareServiceOrdersAsync(Guid customerId, Guid? userId, bool activeOnly, CancellationToken cancellationToken, int page = 1, int limit = 500)
        {
            var orderEntities = await _hardwareServiceOrderRepository.GetAllOrdersAsync(customerId, userId,activeOnly, page, limit, cancellationToken);

            return new PagedModel<HardwareServiceOrderResponseDTO>
            {
                CurrentPage = orderEntities.CurrentPage,
                Items = _mapper.Map<List<HardwareServiceOrderResponseDTO>>(orderEntities.Items),
                PageSize = orderEntities.PageSize,
                TotalItems = orderEntities.TotalItems,
                TotalPages = orderEntities.TotalPages
            };
        }

        public async Task<CustomerSettingsDTO> GetSettingsAsync(Guid customerId)
        {
            var entity = await _hardwareServiceOrderRepository.GetSettingsAsync(customerId);

            var dto = _mapper.Map<CustomerSettingsDTO>(entity) ?? new CustomerSettingsDTO { CustomerId = customerId };

            return dto;
        }

        /// <inheritdoc cref="IHardwareServiceOrderService.UpdateOrderStatusAsync"/>
        public async Task UpdateOrderStatusAsync()
        {
            var customerProviders = await _hardwareServiceOrderRepository.GetAllCustomerProvidersAsync();

            foreach (var customerProvider in customerProviders)
            {
                var provider = await _providerFactory.GetRepairProviderAsync(customerProvider.ServiceProviderId, customerProvider.ApiUserName, customerProvider.ApiPassword);

                var updateStarted = DateTimeOffset.UtcNow;

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

                    // TODO: Fix the new imei list in the DTO
                    await statusHandler.UpdateServiceOrderStatusAsync(origoOrder.ExternalId, lastOrderStatusEnum, new List<string>() { order.ReturnedAsset?.Imei }, order.ReturnedAsset.SerialNumber);

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

        /// <inheritdoc cref="IHardwareServiceOrderService.ConfigureCustomerServiceProviderAsync(int, Guid, string?, string?)"/>
        public async Task<string?> ConfigureCustomerServiceProviderAsync(int providerId, Guid customerId, string? apiUsername, string? apiPassword)
        {
            return await _hardwareServiceOrderRepository.ConfigureCustomerServiceProviderAsync(providerId, customerId, apiUsername, apiPassword);
        }

        /// <inheritdoc cref="IHardwareServiceOrderService.GetServicerProvidersUsernameAsync(Guid, int)"/>
        public async Task<string?> GetServicerProvidersUsernameAsync(Guid customerId, int providerId)
        {
            var serviceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(customerId, providerId);

            return serviceProvider?.ApiUserName;
        }
    }
}
