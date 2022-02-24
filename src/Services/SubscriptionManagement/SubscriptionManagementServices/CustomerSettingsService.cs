using AutoMapper;
using SubscriptionManagementServices.Exceptions;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
using SubscriptionManagementServices.Types;

namespace SubscriptionManagementServices
{
    public class CustomerSettingsService : ICustomerSettingsService
    {
        private readonly IMapper _mapper;
        private readonly ICustomerSettingsRepository _customerSettingsRepository;

        public CustomerSettingsService(ICustomerSettingsRepository customerSettingsRepository, IMapper mapper)
        {
            _mapper = mapper;
            _customerSettingsRepository = customerSettingsRepository;
        }

        public async Task AddOperatorsForCustomerAsync(Guid organizationId, IList<int> operators, Guid callerId)
        {
            await _customerSettingsRepository.AddCustomerOperatorSettingsAsync(organizationId, operators, callerId);
        }

        public async Task<IList<CustomerReferenceFieldDTO>> GetCustomerReferenceFieldsAsync(Guid organizationId)
        {
            var customerReferenceFields = await _customerSettingsRepository.GetCustomerReferenceFieldsAsync(organizationId);
            return _mapper.Map<List<CustomerReferenceFieldDTO>>(customerReferenceFields);
        }

        public async Task<CustomerReferenceFieldDTO> AddCustomerReferenceFieldAsync(Guid organizationId, string name, string type, Guid callerId)
        {
            var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId);
            if (customerSettings == null)
            {
                customerSettings = new CustomerSettings(organizationId, callerId);
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidCustomerReferenceInputDataException("Value for name not set.");
            }
            if (!Enum.TryParse(type, true, out CustomerReferenceTypes customerReferenceFieldType))
            {
                throw new InvalidCustomerReferenceInputDataException("Invalid value for type. Should be 'account' or 'user'.");
            }

            var customerReferenceField = new CustomerReferenceField(name, customerReferenceFieldType, callerId);
            customerSettings.AddCustomerReferenceField(customerReferenceField, callerId);
            if (customerSettings.Id > 0)
            {
                await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSettings);
            }
            else
            {
                await _customerSettingsRepository.AddCustomerSettingsAsync(customerSettings);
            }
            return _mapper.Map<CustomerReferenceFieldDTO>(customerReferenceField);
        }

        public async Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId)
        {
            await _customerSettingsRepository.DeleteOperatorForCustomerAsync(organizationId, operatorId);
        }

        public async Task<CustomerReferenceFieldDTO?> DeleteCustomerReferenceFieldsAsync(Guid organizationId, int customerReferenceFieldId)
        {
            var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId);
            if (customerSettings == null)
            {
                return null;
            }

            var customerReferenceField = customerSettings.RemoveCustomerReferenceField(customerReferenceFieldId);
            if (customerReferenceField == null)
            {
                return null;
            }
            await _customerSettingsRepository.DeleteCustomerReferenceFieldForCustomerAsync(customerReferenceField);
            return _mapper.Map<CustomerReferenceFieldDTO>(customerReferenceField);
        }
    }
}
