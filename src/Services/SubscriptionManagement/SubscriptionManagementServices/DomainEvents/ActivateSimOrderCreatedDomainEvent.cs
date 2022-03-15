using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class ActivateSimOrderCreatedDomainEvent : BaseEvent
    {
        public ActivateSimOrderCreatedDomainEvent(ActivateSimOrder activateSimOrder, Guid callerId) : base(activateSimOrder.SubscriptionOrderId)
        {
            ActivateSimOrder = activateSimOrder;
            CallerId = callerId;
        }

        public ActivateSimOrder ActivateSimOrder { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage()
        {
            return $"Customer with id {ActivateSimOrder.OrganizationId} added a new {ActivateSimOrder.OrderType} for {ActivateSimOrder.MobileNumber}";
        }
    }
}
