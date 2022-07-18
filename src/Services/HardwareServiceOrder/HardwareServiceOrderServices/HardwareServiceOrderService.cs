using AutoMapper;
using Common.Cryptography;
using Common.Interfaces;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using HardwareServiceOrderServices.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace HardwareServiceOrderServices
{
    public class HardwareServiceOrderService : IHardwareServiceOrderService
    {
        private readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;
        private readonly IMapper _mapper;
        private IEmailService _emailService;
        private readonly IProviderFactory _providerFactory;
        private readonly Dictionary<ServiceStatusEnum, ServiceOrderStatusHandlerService> _serviceOrderStatusHandlers;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        public HardwareServiceOrderService(
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IMapper mapper,
            IProviderFactory providerFactory,
            Dictionary<ServiceStatusEnum, ServiceOrderStatusHandlerService> serviceOrderStatusHandlers,
            IEmailService emailService,
            IDataProtectionProvider dataProtectionProvider)
        {
            _hardwareServiceOrderRepository = hardwareServiceOrderRepository;
            _mapper = mapper;
            _emailService = emailService;
            _providerFactory = providerFactory;
            _serviceOrderStatusHandlers = serviceOrderStatusHandlers;
            _dataProtectionProvider = dataProtectionProvider;
        }

        /// <inheritdoc cref="IHardwareServiceOrderService.ConfigureLoanPhoneAsync(Guid, string, string, bool, Guid)"/>
        public async Task<CustomerSettingsDTO> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmail, bool providesLoanDevice, Guid callerId)
        {
            if (providesLoanDevice && string.IsNullOrEmpty(loanPhoneEmail))
                throw new ArgumentException("Loan phone email is required.");

            var entity = await _hardwareServiceOrderRepository.ConfigureLoanPhoneAsync(customerId, loanPhoneNumber, loanPhoneEmail, providesLoanDevice, callerId);

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

        public async Task<HardwareServiceOrderDTO> CreateHardwareServiceOrderAsync(Guid customerId, NewHardwareServiceOrderDTO serviceOrderDTO)
        {
            var customerSetting = await GetSettingsAsync(customerId);

            if (customerSetting == null)
                throw new ArgumentException("Customer settings does not exist");

            var newExternalOrder = new NewExternalRepairOrderDTO(
                                    serviceOrderDTO.OrderedBy.UserId,
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

                var owner = new ContactDetails(serviceOrderDTO.OrderedBy.UserId, serviceOrderDTO.OrderedBy.FirstName, serviceOrderDTO.OrderedBy.LastName, serviceOrderDTO.OrderedBy.Email, serviceOrderDTO.OrderedBy.PhoneNumber);

                var serviceOrder = new HardwareServiceOrder(
                    Guid.NewGuid(),
                    customerId,
                    serviceOrderDTO.AssetInfo.AssetLifecycleId,
                    // TODO: Fix this as we should not have the category id in the child object..
                    (serviceOrderDTO.AssetInfo.AssetCategoryId.HasValue) ? serviceOrderDTO.AssetInfo.AssetCategoryId.Value : 0,
                    new(
                        serviceOrderDTO.AssetInfo.Brand,
                        serviceOrderDTO.AssetInfo.Model,
                        string.IsNullOrEmpty(serviceOrderDTO.AssetInfo.Imei) ? null : new HashSet<string>() { serviceOrderDTO.AssetInfo.Imei }, // TODO: Change this to directly use a ISet
                        serviceOrderDTO.AssetInfo.SerialNumber,
                        serviceOrderDTO.AssetInfo.PurchaseDate,
                        serviceOrderDTO.AssetInfo.Accessories
                    ),
                    serviceOrderDTO.ErrorDescription,
                    owner,
                    deliveryAddress,
                    (int)ServiceTypeEnum.SUR,
                    (int)ServiceStatusEnum.Registered,
                    (int)ServiceProviderEnum.ConmodoNo,
                    externalOrderResponseDTO.ServiceProviderOrderId1,
                    externalOrderResponseDTO.ServiceProviderOrderId2,
                    externalOrderResponseDTO.ExternalServiceManagementLink,
                    new List<ServiceEvent> { }
                    );

                //Creating order at Origo
                var origoOrder = await _hardwareServiceOrderRepository.CreateHardwareServiceOrder(serviceOrder);

                var orderConfirmationMail = new OrderConfirmationEmail
                {
                    AssetId = $"{serviceOrderDTO.AssetInfo.AssetLifecycleId}",
                    AssetName = $"{serviceOrderDTO.AssetInfo.Brand} {serviceOrderDTO.AssetInfo.Model}",
                    FirstName = serviceOrderDTO.OrderedBy.FirstName,
                    OrderDate = origoOrder.DateCreated.UtcDateTime,
                    OrderLink = externalOrderResponseDTO.ExternalServiceManagementLink,
                    Recipient = serviceOrderDTO.OrderedBy.Email,
                    LoanDeviceContact = customerSetting.LoanDevicePhoneNumber,
                    Subject = OrderConfirmationEmail.SubjectKeyName,
                    PackageSlipLink = externalOrderResponseDTO.ExternalServiceManagementLink
                };

                await _emailService.SendOrderConfirmationEmailAsync(orderConfirmationMail, "en");

                var responseDto = _mapper.Map<HardwareServiceOrderDTO>(origoOrder);

                return responseDto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <inheritdoc cref="IHardwareServiceOrderService.GetHardwareServiceOrderAsync(Guid, Guid)"/>
        public async Task<HardwareServiceOrderDTO> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId)
        {
            var orderEntity = await _hardwareServiceOrderRepository.GetOrderAsync(orderId);

            return _mapper.Map<HardwareServiceOrderDTO>(orderEntity);
        }

        public async Task<PagedModel<HardwareServiceOrderDTO>> GetHardwareServiceOrdersAsync(Guid customerId, Guid? userId, bool activeOnly, CancellationToken cancellationToken, int page = 1, int limit = 25)
        {
            var orderEntities = await _hardwareServiceOrderRepository.GetAllOrdersAsync(customerId, userId, activeOnly, page, limit, cancellationToken);

            return new PagedModel<HardwareServiceOrderDTO>
            {
                CurrentPage = orderEntities.CurrentPage,
                Items = _mapper.Map<List<HardwareServiceOrderDTO>>(orderEntities.Items),
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
                string? decryptedApiUserName = await GetServicerProvidersUsernameAsync(customerProvider.CustomerId, customerProvider.ServiceProviderId);
                var provider = await _providerFactory.GetRepairProviderAsync(customerProvider.ServiceProviderId, decryptedApiUserName, customerProvider.ApiPassword);

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
                    await statusHandler.UpdateServiceOrderStatusAsync(origoOrder.ExternalId, lastOrderStatusEnum, new HashSet<string>() { order.ReturnedAsset?.Imei }, order.ReturnedAsset.SerialNumber);

                    //Add events in the log
                    var serviceEvents = _mapper.Map<IEnumerable<ServiceEvent>>(order.ExternalServiceEvents);

                    await _hardwareServiceOrderRepository.UpdateServiceEventsAsync(origoOrder, serviceEvents);
                }

                await _hardwareServiceOrderRepository.UpdateCustomerProviderLastUpdateFetchedAsync(customerProvider, updateStarted);
            }
        }


        /// <inheritdoc cref="IHardwareServiceOrderService.ConfigureCustomerServiceProviderAsync(int, Guid, string?, string?)"/>
        public async Task<string?> ConfigureCustomerServiceProviderAsync(int providerId, Guid customerId, string? apiUsername, string? apiPassword)
        {
            var protector = _dataProtectionProvider.CreateProtector($"{customerId}");

            //Encrypt apiUsername
            apiUsername = string.IsNullOrEmpty(apiUsername) ? apiUsername : protector.Protect(apiUsername);

            //Encrypt apiPassword
            apiPassword = string.IsNullOrEmpty(apiPassword) ? apiPassword : protector.Protect(apiPassword);

            return await _hardwareServiceOrderRepository.ConfigureCustomerServiceProviderAsync(providerId, customerId, apiUsername, apiPassword);
        }

        /// <inheritdoc cref="IHardwareServiceOrderService.GetServicerProvidersUsernameAsync(Guid, int)"/>
        public async Task<string?> GetServicerProvidersUsernameAsync(Guid customerId, int providerId)
        {
            var serviceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(customerId, providerId);

            if (string.IsNullOrEmpty(serviceProvider?.ApiUserName))
                return serviceProvider?.ApiUserName;

            var protector = _dataProtectionProvider.CreateProtector($"{customerId}");

            var decryptedApiUserName = protector.Unprotect(serviceProvider?.ApiUserName);

            return decryptedApiUserName;
        }
    }
}
