using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerOperatorSettingsAddedDomainEvent : BaseEvent
    {
        public CustomerOperatorSettingsAddedDomainEvent(Guid customerId, CustomerOperatorSettings customerOperatorSettings, Guid callerId) : base(customerId)
        {
            CustomerId = customerId;
            CustomerOperatorSettings = customerOperatorSettings;
            CallerId = callerId;
        }
        public Guid CustomerId { get; set; }
        public CustomerOperatorSettings CustomerOperatorSettings { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage()
        {
            return $"Customer operator settings for customer: {CustomerId} was created";
        }
    }
}
