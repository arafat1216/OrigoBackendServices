using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class OrderSimSubscriptionOrderCreatedDomainEvent : BaseEvent
    {
        public OrderSimSubscriptionOrderCreatedDomainEvent(OrderSimSubscriptionOrder orderSimSubscriptionOrder, Guid callerId): base(orderSimSubscriptionOrder.SubscriptionOrderId)
        {
            OrderSimSubscriptionOrder = orderSimSubscriptionOrder;
            CallerId = callerId;
        }

        public OrderSimSubscriptionOrder OrderSimSubscriptionOrder { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer with id {OrderSimSubscriptionOrder.OrganizationId} added a new {OrderSimSubscriptionOrder.OrderType} for {OrderSimSubscriptionOrder.SendToName} with operator {OrderSimSubscriptionOrder.OperatorName}";
        }
    }
}
