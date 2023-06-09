﻿using AutoMapper;
using Common.Interfaces;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Exceptions;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using Microsoft.AspNetCore.DataProtection;

namespace HardwareServiceOrderServices
{
    public class HardwareServiceOrderService : IHardwareServiceOrderService
    {
        private readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IProviderFactory _providerFactory;
        private readonly IStatusHandlerFactory _statusHandlerFactory;


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
            IEmailService emailService)
        {
            _hardwareServiceOrderRepository = hardwareServiceOrderRepository;
            _mapper = mapper;
            _emailService = emailService;
            _providerFactory = providerFactory;
            _statusHandlerFactory = statusHandlerFactory;
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

            var customerProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(customerId, serviceOrderDTO.ServiceProviderId, true, true);

            HashSet<string> serviceOrderAddonIds = new HashSet<string>();
            if (customerProvider?.ActiveServiceOrderAddons is not null && serviceOrderDTO.UserSelectedServiceOrderAddonIds.Count > 0)
            {
                foreach (var serviceOrderAddonId in serviceOrderDTO.UserSelectedServiceOrderAddonIds)
                {
                    var serviceOrderAddon = customerProvider?.ActiveServiceOrderAddons.FirstOrDefault(serviceOrderAddon => serviceOrderAddon.IsUserSelectable && serviceOrderAddon.Id == serviceOrderAddonId);
                    if (serviceOrderAddon is null)
                        throw new ArgumentException($"Wrong Service Addon Id provided for customer {customerId}, Service Order Addon: {serviceOrderAddonId}", nameof(customerId));
                    serviceOrderAddonIds.Add(serviceOrderAddonId.ToString());
                }

                var nonUserSelectableServiceOrderAddons = customerProvider?.ActiveServiceOrderAddons.Where(serviceOrderAddon => !serviceOrderAddon.IsUserSelectable);
                if (nonUserSelectableServiceOrderAddons is not null)
                {
                    foreach (var serviceOrderAddon in nonUserSelectableServiceOrderAddons)
                    {
                        serviceOrderAddonIds.Add(serviceOrderAddon.Id.ToString());
                    }
                }
            }

            // Need to add the Service addons
            var newExternalServiceOrder = new NewExternalServiceOrderDTO(
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
                                    serviceOrderDTO.UserDescription,
                                    serviceOrderAddonIds);

            if (customerProvider == null)
                throw new ArgumentException($"Service provider is not configured for customer {customerId}", nameof(customerId));

            if (customerProvider?.ApiCredentials == null || customerProvider?.ApiCredentials.Count == 0)
                throw new ArgumentException($"API Credential does not exist for customer {customerId}, ServiceType: {serviceOrderDTO.ServiceTypeId}", nameof(customerId));

            var apiCredential = customerProvider?.ApiCredentials.FirstOrDefault(apiCred => apiCred.ServiceTypeId == serviceOrderDTO.ServiceTypeId) ??
                                           customerProvider?.ApiCredentials.FirstOrDefault(apiCred => apiCred.ServiceTypeId == null);

            if (apiCredential == null)
                throw new ArgumentException($"API Credential does not exist for customer {customerId}, ServiceType: {serviceOrderDTO.ServiceTypeId}", nameof(customerId));

            //Todo: Need to do some validation checking for apiCredential ApiUsername and ApiPassword

            var newOrderId = Guid.NewGuid();

            // ServiceTypeId refers to the values of ServiceTypeEnum, like SUR, Remarketing
            NewExternalServiceOrderResponseDTO externalOrderResponseDTO;

            switch (serviceOrderDTO.ServiceTypeId)
            {
                case (int)ServiceTypeEnum.SUR:
                    {
                        var repairProvider = await _providerFactory.GetRepairProviderAsync(serviceOrderDTO.ServiceProviderId,
                            _hardwareServiceOrderRepository.Decrypt(apiCredential?.ApiUsername, customerId.ToString()),
                            _hardwareServiceOrderRepository.Decrypt(apiCredential?.ApiPassword, customerId.ToString()));
                        externalOrderResponseDTO = await repairProvider.CreateRepairOrderAsync(newExternalServiceOrder, serviceOrderDTO.ServiceTypeId, $"{newOrderId}");
                        break;
                    }
                case (int)ServiceTypeEnum.Remarketing:
                    {
                        var aftermarketProvider = await _providerFactory.GetAftermarketProviderAsync(serviceOrderDTO.ServiceProviderId,
                            _hardwareServiceOrderRepository.Decrypt(apiCredential?.ApiUsername, customerId.ToString()),
                            _hardwareServiceOrderRepository.Decrypt(apiCredential?.ApiPassword, customerId.ToString()));
                        externalOrderResponseDTO = await aftermarketProvider.CreateAftermarketOrderAsync(newExternalServiceOrder, serviceOrderDTO.ServiceTypeId, $"{newOrderId}");
                        break;
                    }
                default:
                    throw new NotSupportedException($"The requested ServiceTypeId: {serviceOrderDTO.ServiceTypeId} is either not supported or wrong ServiceTypeId");
            }

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
                serviceOrderDTO.UserDescription,
                owner,
                deliveryAddress,
                serviceOrderDTO.ServiceTypeId, // ServiceTypeId refers to the values of ServiceTypeEnum, like SUR, Remarketing
                (int)ServiceStatusEnum.Registered,
                serviceOrderDTO.ServiceProviderId,
                new HashSet<int>(serviceOrderAddonIds.ToList().ConvertAll(x => (int.Parse(x)))),
                externalOrderResponseDTO.ServiceProviderOrderId1,
                externalOrderResponseDTO.ServiceProviderOrderId2,
                externalOrderResponseDTO.ExternalServiceManagementLink,
                new List<ServiceEvent> { }
                );

            //Creating order at Origo
            var origoOrder = await _hardwareServiceOrderRepository.CreateHardwareServiceOrderAsync(serviceOrder);

            // Preparing related events
            var statusHandler = _statusHandlerFactory.GetStatusHandler((ServiceTypeEnum)serviceOrder.ServiceTypeId);
            await statusHandler.HandleServiceOrderRegisterAsync(origoOrder, serviceOrderDTO, newExternalServiceOrder, externalOrderResponseDTO, customerSetting);

            var responseDto = _mapper.Map<HardwareServiceOrderDTO>(origoOrder);

            return responseDto;

        }


        /// <inheritdoc/>
        public async Task<HardwareServiceOrderDTO?> GetServiceOrderByIdAsync(Guid serviceOrderId, Guid? organizationId = null)
        {
            var orderEntity = await _hardwareServiceOrderRepository.GetServiceOrderByIdAsync(serviceOrderId, true, organizationId: organizationId);

            return _mapper.Map<HardwareServiceOrderDTO?>(orderEntity);
        }


        /// <inheritdoc/>
        public async Task<PagedModel<HardwareServiceOrderDTO>> GetAllServiceOrdersForOrganizationAsync(Guid organizationId, Guid? userId, int? serviceTypeId, bool activeOnly, CancellationToken cancellationToken, int page = 1, int limit = 25, Guid? assetLifecycleId = null, ISet<int>? statusIds = null, string? search = null)
        {
            var orderEntities = await _hardwareServiceOrderRepository.GetAllServiceOrdersForOrganizationAsync(organizationId, userId, serviceTypeId, activeOnly, page, limit, true, cancellationToken, assetLifecycleId: assetLifecycleId, statusIds: statusIds, search: search);

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
            var customerServiceProviders = await _hardwareServiceOrderRepository.GetCustomerServiceProvidersByFilterAsync(null, true, false, true);

            foreach (var customerServiceProvider in customerServiceProviders)
            {
                var apiCredentials = customerServiceProvider?.ApiCredentials;
                if (apiCredentials is null || apiCredentials.Count == 0)
                    continue;

                foreach (var apiCredential in apiCredentials)
                {
                    var provider = await _providerFactory.GetRepairProviderAsync(customerServiceProvider.ServiceProviderId,
                        _hardwareServiceOrderRepository.Decrypt(apiCredential?.ApiUsername, customerServiceProvider.CustomerId.ToString()),
                        _hardwareServiceOrderRepository.Decrypt(apiCredential?.ApiPassword, customerServiceProvider.CustomerId.ToString()));

                    var updateStarted = DateTimeOffset.UtcNow;

                    // Todo: Need to think about a default "apiCredential.LastUpdateFetched"
                    var updatedExternalOrders = await provider.GetOrdersUpdatedSinceAsync(apiCredential?.LastUpdateFetched ?? updateStarted);

                    foreach (var externalOrder in updatedExternalOrders)
                    {
                        if (!externalOrder.ExternalServiceEvents.Any())
                            continue;

                        var lastOrderStatus = externalOrder.ExternalServiceEvents.OrderByDescending(m => m.Timestamp).FirstOrDefault()?.ServiceStatusId;

                        // Todo: GetOrderByServiceProviderOrderIdAsync will be replaced by which method
                        var origoOrder = await _hardwareServiceOrderRepository.GetOrderByServiceProviderOrderIdAsync(externalOrder.ServiceProviderOrderId1);

                        if (origoOrder == null || origoOrder.StatusId == lastOrderStatus)
                            continue;

                        // TODO: Fix the new imei list in the DTO
                        var statusHandler = _statusHandlerFactory.GetStatusHandler((ServiceTypeEnum)origoOrder.ServiceTypeId);
                        await statusHandler.HandleServiceOrderStatusAsync(origoOrder, externalOrder);

                        //Add events in the log
                        var serviceEvents = _mapper.Map<IEnumerable<ServiceEvent>>(externalOrder.ExternalServiceEvents);

                        await _hardwareServiceOrderRepository.UpdateServiceEventsAsync(origoOrder, serviceEvents);
                    }

                    await _hardwareServiceOrderRepository.UpdateApiCredentialLastUpdateFetchedAsync(apiCredential!, updateStarted);
                }
            }
        }


        /// <inheritdoc/>
        public async Task<string?> ConfigureCustomerServiceProviderAsync(int providerId, Guid customerId, string? apiUsername, string? apiPassword)
        {
            return await _hardwareServiceOrderRepository.ConfigureCustomerServiceProviderAsync(providerId, customerId, apiUsername, apiPassword);
        }


        /// <inheritdoc/>
        public async Task<string?> GetServicerProvidersUsernameAsync(Guid customerId, int providerId)
        {
            var serviceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(customerId, providerId, false, false);

            var decryptedApiUserName = _hardwareServiceOrderRepository.Decrypt(serviceProvider?.ApiUserName, customerId.ToString());

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
        public async Task<ServiceProviderDTO> GetServiceProviderById(int id, bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons)
        {
            ServiceProvider? serviceProvider = await _hardwareServiceOrderRepository.GetServiceProviderByIdAsync(id, includeSupportedServiceTypes, includeOfferedServiceOrderAddons, true);

            if (serviceProvider is null)
                throw new NotFoundException();

            var serviceProviderDTO = _mapper.Map<ServiceProviderDTO>(serviceProvider);
            return serviceProviderDTO;
        }


        /// <inheritdoc/>
        public async Task DeleteApiCredentialAsync(Guid organizationId, int serviceProviderId, int? serviceTypeId)
        {
            var customerServiceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(organizationId, serviceProviderId, true, false);
            ApiCredential? apiCredentialToBeRemoved = customerServiceProvider?.ApiCredentials?.FirstOrDefault(e => e.ServiceTypeId == serviceTypeId);

            // If the credentials was not found (don't exist), do a return so the API considers it "deleted".
            if (apiCredentialToBeRemoved is null)
                return;

            await _hardwareServiceOrderRepository.DeleteAndSaveAsync(apiCredentialToBeRemoved);
        }


        /// <inheritdoc/>
        public async Task AddOrUpdateApiCredentialAsync(Guid organizationId, int serviceProviderId, int? serviceTypeId, string? apiUsername, string? apiPassword)
        {
            // Currently "ServiceType--> SUR(aka Repair), Remarketing" are supported only
            if (serviceTypeId is not null && (ServiceTypeEnum.SUR != (ServiceTypeEnum)serviceTypeId && ServiceTypeEnum.Remarketing != (ServiceTypeEnum)serviceTypeId))
                throw new NotImplementedException("Requested ServiceType is currently not supported.");

            var customerServiceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(organizationId, serviceProviderId, false, false);

            if (customerServiceProvider is null)
            {
                // What do you mean it don't exist?
                customerServiceProvider = new(organizationId, serviceProviderId, null, null);
                customerServiceProvider = await _hardwareServiceOrderRepository.AddAndSaveAsync(customerServiceProvider);
                // Ahh! So that's where I put it. I told you it existed ;)
            }

            await _hardwareServiceOrderRepository.AddOrUpdateApiCredentialAsync(organizationId, customerServiceProvider.Id, serviceTypeId, apiUsername, apiPassword);
        }


        /// <inheritdoc/>
        public async Task<IEnumerable<CustomerServiceProviderDto>> GetCustomerServiceProvidersAsync(Guid organizationId, bool includeApiCredentials = false, bool includeActiveServiceOrderAddons = false)
        {
            var customerServiceProviders = await _hardwareServiceOrderRepository.GetCustomerServiceProvidersByFilterAsync(entity => entity.CustomerId == organizationId, includeApiCredentials, includeActiveServiceOrderAddons, true);

            var mapped = _mapper.Map<IEnumerable<CustomerServiceProviderDto>>(customerServiceProviders);
            return mapped;
        }


        /// <inheritdoc/>
        public async Task<CustomerServiceProviderDto> GetCustomerServiceProviderAsync(Guid organizationId, int serviceProviderId, bool includeApiCredentials = false, bool includeActiveServiceOrderAddons = false)
        {
            CustomerServiceProvider? customerServiceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(organizationId, serviceProviderId, includeApiCredentials, includeActiveServiceOrderAddons);
            if (customerServiceProvider is null)
                throw new NotFoundException("The customer service provider was not found.");

            CustomerServiceProviderDto mapped = _mapper.Map<CustomerServiceProviderDto>(customerServiceProvider);
            return mapped;
        }


        /// <inheritdoc/>
        public async Task AddServiceOrderAddonsToCustomerServiceProviderAsync(Guid organizationId, int serviceProviderId, ISet<int> newServiceOrderAddonIds)
        {
            CustomerServiceProvider? customerServiceProvider = await _hardwareServiceOrderRepository.GetCustomerServiceProviderAsync(organizationId, serviceProviderId, false, true);
            var retrievedServiceOrderAddons = await _hardwareServiceOrderRepository.GetByIdAsync<ServiceOrderAddon>(newServiceOrderAddonIds, false);

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
        public async Task RemoveServiceOrderAddonsFromCustomerServiceProviderAsync(Guid organizationId, int serviceProviderId, ISet<int> removedServiceOrderAddonIds)
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

        /// <inheritdoc/>
        public async Task<CustomerSettingsDTO> AddOrUpdateCustomerSettings(CustomerSettingsDTO customerSettingsDTO)
        {
            CustomerSettings result;
            var existingCustomerSettings = await _hardwareServiceOrderRepository.GetCustomerSettingsByOrganizationIdAsync(customerSettingsDTO.CustomerId);

            if (existingCustomerSettings is null)
            {
                CustomerSettings customerSettings = new(customerSettingsDTO.CustomerId, customerSettingsDTO.ProvidesLoanDevice, customerSettingsDTO.LoanDevicePhoneNumber, customerSettingsDTO.LoanDeviceEmail);
                result = await _hardwareServiceOrderRepository.AddAndSaveAsync(customerSettings);
            }
            else
            {
                existingCustomerSettings.ProvidesLoanDevice = customerSettingsDTO.ProvidesLoanDevice;
                existingCustomerSettings.LoanDevicePhoneNumber = customerSettingsDTO.LoanDevicePhoneNumber;
                existingCustomerSettings.LoanDeviceEmail = customerSettingsDTO.LoanDeviceEmail;

                result = await _hardwareServiceOrderRepository.UpdateAndSaveAsync(existingCustomerSettings);
            }

            return _mapper.Map<CustomerSettingsDTO>(result);
        }


        /// <inheritdoc/>
        public async Task<CustomerSettingsDTO?> GetCustomerSettings(Guid organizationId)
        {
            var result = await _hardwareServiceOrderRepository.GetCustomerSettingsByOrganizationIdAsync(organizationId);
            return _mapper.Map<CustomerSettingsDTO?>(result);
        }
    }
}
