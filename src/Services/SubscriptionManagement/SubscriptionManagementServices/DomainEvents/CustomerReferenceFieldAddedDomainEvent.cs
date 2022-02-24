using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerReferenceFieldAddedDomainEvent : BaseEvent
    {
        public CustomerReferenceFieldAddedDomainEvent(Guid customerId, CustomerReferenceField customerReferenceField, Guid callerId) : base(customerId)
        {
            CustomerId = customerId;
            CustomerReferenceField = customerReferenceField;
            CallerId = callerId;
        }

        public Guid CustomerId { get; set; }
        public CustomerReferenceField CustomerReferenceField { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer reference field {CustomerReferenceField.Name} added for customer id {CustomerId}";
        }
    }
}
