
using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class NewSubscriptionOrderCreatedDomainEvent : BaseEvent
    {
        public NewSubscriptionOrderCreatedDomainEvent(NewSubscriptionOrder newSubscriptionOrder, Guid callerId) : base(newSubscriptionOrder.SubscriptionOrderId)
        {
            NewSubscriptionOrder = newSubscriptionOrder;
            CallerId = callerId;
        }

        public NewSubscriptionOrder NewSubscriptionOrder { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage()
        {
            return $"Customer with id {NewSubscriptionOrder.OrganizationId} added a {NewSubscriptionOrder.OrderType} order for mobile number {NewSubscriptionOrder.MobileNumber}";
        }
    }
}
