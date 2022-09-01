using AutoMapper;
using Common.Interfaces;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Exceptions;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace HardwareServiceOrderServices
{
    public class HardwareServiceOrderService : IHardwareServiceOrderService
    {
        private readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IProviderFactory _providerFactory;
        private readonly IStatusHandlerFactory _statusHandlerFactory;
        private readonly IDataProtectionProvider _dataProtectionProvider;


        /// <summary>
        ///     Initializes a new instance of the <see cref="HardwareServiceOrderService"/>-class using dependency-injection.
        /// </summary>
        /// <param name="hardwareServiceOrderRepository"> A dependency-injected implementation of the <see cref="IHardwareServiceOrderRepository"/> interface. </param>
        /// <param name="mapper"> A dependency-injected implementation of the <see cref="IMapper"/> interface. </param>
        /// <param name="providerFactory"> A dependency-injected implementation of the <see cref="IProviderFactory"/> interface. </param>
        /// <param name="statusHandlerFactory"> A dependency-injected implementation of the <see cref="IStatusHandlerFactory"/> interface. </param>
        /// <param name="emailService"> A dependency-injected implementation of the <see cref="IEmailService"/> interface. </param>
        /// <param name="dataProtectionProvider"> A dependency-injected implementation of the <see cref="IDataProtectionProvider"/> interface. </param>
        public HardwareServiceOrderService(
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IMapper mapper,
            IProviderFactory providerFactory,
            IStatusHandlerFactory statusHandlerFactory,
            IEmailService emailService,
            IDataProtectionProvider dataProtectionProvider)
        {
            _hardwareServiceOrderRepository = hardwareServiceOrderRepository;
            _mapper = mapper;
            _emailService = emailService;
            _providerFactory = providerFactory;
            _statusHandlerFactory = statusHandlerFactory;
            _dataProtectionProvider = dataProtectionProvider;
        }


        /// <inheritdoc/>
        public async Task<CustomerSettingsDTO> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmail, bool providesLoanDevice, Guid callerId)
        {
            if (providesLoanDevice && string.IsNullOrEmpty(loanPhoneEmail))
                throw new ArgumentException("Loan phone email is required.");

            var entity = await _hardwareServiceOrderRepository.ConfigureLoanPhoneAsync(customerId, loanPhoneNumber, loanPhoneEmail, providesLoanDevice, callerId);

            var dto = _mapper.Map<CustomerSettingsDTO>(entity);

            return dto;
        }


        /// <inheritdoc/>
        public async Task<CustomerSettingsDTO> ConfigureCustomerSettingsAsync(Guid customerId, Guid callerId)
        {
            var entity = await _hardwareServiceOrderRepository.ConfigureCustomerSettingsAsync(customerId, callerId);

            var dto = _mapper.Map<CustomerSettingsDTO>(entity);

            return dto;
        }


        /// <inheritdoc/>
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

            var customerProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(customerId, (int)ServiceProviderEnum.ConmodoNo, false, false);

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
                    serviceOrderDTO.AssetInfo.AssetCategoryId ?? 0,
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
                var origoOrder = await _hardwareServiceOrderRepository.CreateHardwareServiceOrderAsync(serviceOrder);

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


        /// <inheritdoc/>
        public async Task<HardwareServiceOrderDTO> GetHardwareServiceOrderAsync(Guid customerId, Guid orderId)
        {
            var orderEntity = await _hardwareServiceOrderRepository.GetServiceOrderAsync(orderId);

            return _mapper.Map<HardwareServiceOrderDTO>(orderEntity);
        }


        /// <inheritdoc/>
        public async Task<PagedModel<HardwareServiceOrderDTO>> GetHardwareServiceOrdersAsync(Guid customerId, Guid? userId, bool activeOnly, CancellationToken cancellationToken, int page = 1, int limit = 25)
        {
            var orderEntities = await _hardwareServiceOrderRepository.GetAllServiceOrdersAsync(customerId, userId, activeOnly, page, limit, cancellationToken);

            return new PagedModel<HardwareServiceOrderDTO>
            {
                CurrentPage = orderEntities.CurrentPage,
                Items = _mapper.Map<List<HardwareServiceOrderDTO>>(orderEntities.Items),
                PageSize = orderEntities.PageSize,
                TotalItems = orderEntities.TotalItems,
                TotalPages = orderEntities.TotalPages
            };
        }


        /// <inheritdoc/>
        public async Task<CustomerSettingsDTO> GetSettingsAsync(Guid customerId)
        {
            var entity = await _hardwareServiceOrderRepository.GetSettingsAsync(customerId);

            var dto = _mapper.Map<CustomerSettingsDTO>(entity) ?? new CustomerSettingsDTO { CustomerId = customerId };

            return dto;
        }


        /// <inheritdoc/>
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

                    // TODO: Fix the new imei list in the DTO
                    var statusHandler = _statusHandlerFactory.GetStatusHandler(ServiceTypeEnum.SUR);
                    await statusHandler.HandleServiceOrderStatusAsync(origoOrder, order);

                    //Add events in the log
                    var serviceEvents = _mapper.Map<IEnumerable<ServiceEvent>>(order.ExternalServiceEvents);

                    await _hardwareServiceOrderRepository.UpdateServiceEventsAsync(origoOrder, serviceEvents);
                }

                await _hardwareServiceOrderRepository.UpdateCustomerProviderLastUpdateFetchedAsync(customerProvider, updateStarted);
            }
        }


        /// <inheritdoc/>
        public async Task<string?> ConfigureCustomerServiceProviderAsync(int providerId, Guid customerId, string? apiUsername, string? apiPassword)
        {
            var protector = _dataProtectionProvider.CreateProtector($"{customerId}");

            //Encrypt apiUsername
            apiUsername = string.IsNullOrEmpty(apiUsername) ? apiUsername : protector.Protect(apiUsername);

            //Encrypt apiPassword
            apiPassword = string.IsNullOrEmpty(apiPassword) ? apiPassword : protector.Protect(apiPassword);

            return await _hardwareServiceOrderRepository.ConfigureCustomerServiceProviderAsync(providerId, customerId, apiUsername, apiPassword);
        }


        /// <inheritdoc/>
        public async Task<string?> GetServicerProvidersUsernameAsync(Guid customerId, int providerId)
        {
            var serviceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(customerId, providerId, false, false);

            if (string.IsNullOrEmpty(serviceProvider?.ApiUserName))
                return serviceProvider?.ApiUserName;

            var protector = _dataProtectionProvider.CreateProtector($"{customerId}");

            var decryptedApiUserName = protector.Unprotect(serviceProvider?.ApiUserName);

            return decryptedApiUserName;
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<ServiceProviderDTO>> GetAllServiceProvidersAsync(bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons)
        {
            var serviceProviders = await _hardwareServiceOrderRepository.GetAllServiceProvidersAsync(includeSupportedServiceTypes, includeOfferedServiceOrderAddons, true);
            var serviceProviderDTOs = _mapper.Map<IEnumerable<ServiceProviderDTO>>(serviceProviders);

            return serviceProviderDTOs;
        }


        /// <inheritdoc/>
        public async Task DeleteApiCredentialAsync(Guid organizationId, int serviceProviderId, int? serviceTypeId)
        {
            var customerServiceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(organizationId, serviceProviderId, true, false);
            ApiCredential? apiCredentialToBeRemoved = customerServiceProvider?.ApiCredentials?.FirstOrDefault(e => e.ServiceTypeId == serviceTypeId);

            // If the credentials was not found (don't exist), do a return so the API considers it "deleted".
            if (apiCredentialToBeRemoved is null)
                return;

            await _hardwareServiceOrderRepository.Delete(apiCredentialToBeRemoved);
        }


        /// <inheritdoc/>
        public async Task AddOrUpdateApiCredentialAsync(Guid organizationId, int serviceProviderId, int? serviceTypeId, string? apiUsername, string? apiPassword)
        {
            var customerServiceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(organizationId, serviceProviderId, false, false);

            if (customerServiceProvider is null)
            {
                // What do you mean it don't exist?
                customerServiceProvider = new(organizationId, serviceProviderId, null, null);
                customerServiceProvider = await _hardwareServiceOrderRepository.AddAndSaveAsync(customerServiceProvider);
                // Ahh! So that's where I put it. I told you it existed ;)
            }

            await _hardwareServiceOrderRepository.AddOrUpdateApiCredentialAsync(customerServiceProvider.Id, serviceTypeId, apiUsername, apiPassword);
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<CustomerServiceProviderDto>> GetCustomerServiceProvidersAsync(Guid organizationId, bool includeApiCredentials = false, bool includeActiveServiceOrderAddons = false)
        {
            var customerServiceProviders = await _hardwareServiceOrderRepository.GetCustomerServiceProvidersByFilterAsync(entity => entity.CustomerId == organizationId, includeApiCredentials, includeActiveServiceOrderAddons, true);

            var mapped = _mapper.Map<IEnumerable<CustomerServiceProviderDto>>(customerServiceProviders);
            return mapped;
        }


        /// <inheritdoc/>
        public async Task<CustomerServiceProviderDto> GetCustomerServiceProviderByIdAsync(Guid organizationId, int serviceProviderId, bool includeApiCredentials = false, bool includeActiveServiceOrderAddons = false)
        {
            CustomerServiceProvider? customerServiceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(organizationId, serviceProviderId, includeApiCredentials, includeActiveServiceOrderAddons);
            if (customerServiceProvider is null)
                throw new NotFoundException("The customer service provider was not found.");

            CustomerServiceProviderDto mapped = _mapper.Map<CustomerServiceProviderDto>(customerServiceProvider);
            return mapped;
        }


        /// <inheritdoc/>
        public async Task AddServiceOrderAddonsToCustomerServiceProvider(Guid organizationId, int serviceProviderId, ISet<int> newServiceOrderAddonIds)
        {
            CustomerServiceProvider? customerServiceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(organizationId, serviceProviderId, false, true);
            var retrievedServiceOrderAddons = await _hardwareServiceOrderRepository.GetByIdAsync<ServiceOrderAddon>(newServiceOrderAddonIds);

            // Let's make sure all requested items actually exist (was found)
            if (newServiceOrderAddonIds.Count != retrievedServiceOrderAddons.Count())
                throw new NotFoundException("One or more of the requested service-order addons was not found!");

            // Let's ensure the new service-order addons is actually valid for this service-provider!
            foreach (var addon in retrievedServiceOrderAddons)
            {
                if (addon.ServiceProviderId != serviceProviderId)
                    throw new ArgumentException("You are trying to add a service-order addon that don't exist, or is unavailable for this service-provider.");
            }

            if (customerServiceProvider is null)
            {
                // What do you mean it don't exist?
                customerServiceProvider = new(organizationId, serviceProviderId, null, retrievedServiceOrderAddons.ToList());
                await _hardwareServiceOrderRepository.AddAndSaveAsync(customerServiceProvider);
                // Ahh! So that's where I put it. I told you it existed ;)
            }
            else
            {
                // Extract the missing IDs
                var activeServiceOrderAddonIds = customerServiceProvider.ActiveServiceOrderAddons!.Select(e => e.Id);
                var missingServiceOrderAddonIds = newServiceOrderAddonIds.Except(activeServiceOrderAddonIds);

                if (missingServiceOrderAddonIds.Any())
                {
                    foreach (var currentAddon in retrievedServiceOrderAddons)
                    {
                        // If it exist in the missing it list, add it.
                        if (missingServiceOrderAddonIds.Any(i => currentAddon.Id == i))
                            customerServiceProvider.ActiveServiceOrderAddons!.Add(currentAddon);
                    }

                    await _hardwareServiceOrderRepository.UpdateAndSaveAsync(customerServiceProvider);
                }
            }
        }


        /// <inheritdoc/>
        public async Task RemoveServiceOrderAddonsFromCustomerServiceProvider(Guid organizationId, int serviceProviderId, ISet<int> removedServiceOrderAddonIds)
        {
            CustomerServiceProvider? customerServiceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(organizationId, serviceProviderId, false, true);

            // If it's null, then it don't exist meaning we don't have anything to remove from. Let's do a return so the API considers it "deleted".
            if (customerServiceProvider is null)
                return;

            foreach (var currentAddon in customerServiceProvider.ActiveServiceOrderAddons!)
            {
                if (removedServiceOrderAddonIds.Any(i => currentAddon.Id == i))
                    customerServiceProvider.ActiveServiceOrderAddons.Remove(currentAddon);
            }

            await _hardwareServiceOrderRepository.UpdateAndSaveAsync(customerServiceProvider);
        }
    }
}
