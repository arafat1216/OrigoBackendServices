using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerOperatorSettingsRemovedDomainEvent : BaseEvent
    {
        public CustomerOperatorSettingsRemovedDomainEvent(Guid customerId,CustomerOperatorSettings customerOperatorSettings, int operatorId) : base(customerId)
        {
            CustomerOperatorSettings = customerOperatorSettings;
            CustomerId = customerId;
            OperatorId = operatorId;
        }

        public CustomerOperatorSettings CustomerOperatorSettings { get; set; }
        public Guid CustomerId { get; set; }
        public int OperatorId { get; set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer operator setting for operator id {OperatorId} for customer {CustomerId} was removed";
        }
    }
}
