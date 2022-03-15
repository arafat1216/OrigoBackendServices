using Common.Logging;
using SubscriptionManagementServices.Models;


namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerReferenceFieldRemovedDomainEvent : BaseEvent
    {
        public CustomerReferenceFieldRemovedDomainEvent(CustomerReferenceField customerReferenceField, Guid customerId) : base(customerId)
        {
            CustomerReferenceField = customerReferenceField;
            CustomerId = customerId;
        }

        public CustomerReferenceField CustomerReferenceField { get; set; }
        public Guid CustomerId { get; set; }
        public override string EventMessage()
        {
            return $"Customer reference field {CustomerReferenceField.Name} was removed for customer id {CustomerId}";
        }
    }
}
