using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerSettings : Entity, IAggregateRoot
    {
        private List<CustomerOperatorSettings> _customerOperatorSettings;
        public CustomerSettings(Guid customerId, IReadOnlyCollection<CustomerReferenceField>? customerReferenceFields)
        {
            CustomerId = customerId;
            CustomerReferenceFields = customerReferenceFields;
            _customerOperatorSettings = new List<CustomerOperatorSettings>();
        }

        public CustomerSettings(Guid customerId)
        {
            CustomerId = customerId;
            _customerOperatorSettings = new List<CustomerOperatorSettings>();
        }

        public CustomerSettings()
        {
            _customerOperatorSettings = new List<CustomerOperatorSettings>();
        }

        public Guid CustomerId { get; protected set; }
        public IReadOnlyCollection<CustomerOperatorSettings>? CustomerOperatorSettings => _customerOperatorSettings.AsReadOnly();

        public IReadOnlyCollection<CustomerReferenceField>? CustomerReferenceFields { get; protected set; }

        public void UpdateCustomerOperatorSettings(List<CustomerOperatorSettings> customerOperatorSettings)
        {
            _customerOperatorSettings.AddRange(customerOperatorSettings);
        }

        public void AddCustomerOperatorSettings(List<CustomerOperatorSettings> customerOperatorSettings)
        {
            _customerOperatorSettings.AddRange(customerOperatorSettings);
        }

        public void RemoveCustomerOperatorSettings(int operatorId)
        {
            var customerOperatorSettings = _customerOperatorSettings.FirstOrDefault(x => x.OperatorId == operatorId);

            if (customerOperatorSettings != null)
            {
                _customerOperatorSettings.Remove(customerOperatorSettings);
                return;
            }

            throw new ArgumentException($"Operator {operatorId} is not associated with this customer.");
        }
    }
}
