using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class CustomerSettings : Entity, IAggregateRoot
    {

        public CustomerSettings(Guid customerId, IReadOnlyCollection<CustomerOperatorSettings>? customerOperatorSettings, IReadOnlyCollection<CustomerReferenceField>? customerReferenceFields)
        {
            CustomerId = customerId;
            CustomerOperatorSettings = customerOperatorSettings;
            CustomerReferenceFields = customerReferenceFields;
        }

        public CustomerSettings(Guid customerId, IReadOnlyCollection<CustomerOperatorSettings>? customerOperatorSettings)
        {
            CustomerId = customerId;
            CustomerOperatorSettings = customerOperatorSettings;
        }

        public CustomerSettings()
        {
            //CustomerId = customerId;
        }
        
        public Guid CustomerId { get; protected set; }
        public IReadOnlyCollection<CustomerOperatorSettings>? CustomerOperatorSettings { get; protected set; }
        public IReadOnlyCollection<CustomerReferenceField>? CustomerReferenceFields { get; protected set; }
    }
}
