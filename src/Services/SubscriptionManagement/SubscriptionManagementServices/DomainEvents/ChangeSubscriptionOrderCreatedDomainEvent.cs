using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class ChangeSubscriptionOrderCreatedDomainEvent : BaseEvent
    {
        public ChangeSubscriptionOrderCreatedDomainEvent(ChangeSubscriptionOrder changeSubscriptionOrder, Guid callerId): base(changeSubscriptionOrder.SubscriptionOrderId)
        {
            ChangeSubscriptionOrder = changeSubscriptionOrder;
            CallerId = callerId;
        }

        public ChangeSubscriptionOrder ChangeSubscriptionOrder { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer with id {ChangeSubscriptionOrder.OrganizationId} added a new {ChangeSubscriptionOrder.OrderType} for employee {ChangeSubscriptionOrder.SubscriptionOwner}. Changed subscription to {ChangeSubscriptionOrder.ProductName} for operator {ChangeSubscriptionOrder.OperatorName}";
        }
    }
}
