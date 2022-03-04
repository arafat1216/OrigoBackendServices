using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CancelSubscriptionOrderCreatedDomainEvent : BaseEvent
    {
        public CancelSubscriptionOrderCreatedDomainEvent(CancelSubscriptionOrder cancelSubscriptionOrder, Guid callerId): base(cancelSubscriptionOrder.SubscriptionOrderId)
        {
            CancelSubscriptionOrder = cancelSubscriptionOrder;
            CallerId = callerId;
        }

        public CancelSubscriptionOrder CancelSubscriptionOrder { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer with id {CancelSubscriptionOrder.OrganizationId} added a new {CancelSubscriptionOrder.OrderType} for {CancelSubscriptionOrder.MobileNumber} with operator {CancelSubscriptionOrder.OperatorName}";
        }
    }
}
