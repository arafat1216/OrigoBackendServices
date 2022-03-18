using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerStandardPrivateSubscriptionProductAddedDomainEvent : BaseEvent
    {
        public CustomerStandardPrivateSubscriptionProductAddedDomainEvent(CustomerStandardPrivateSubscriptionProduct customerStandardPrivateSubscriptionProduct, Guid callerId)
        {
            CustomerStandardPrivateSubscriptionProduct = customerStandardPrivateSubscriptionProduct;
            CallerId = callerId;
        }

        public CustomerStandardPrivateSubscriptionProduct CustomerStandardPrivateSubscriptionProduct { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage()
        {
            return $"Customer id: {CustomerStandardPrivateSubscriptionProduct.OrganizationId} added {CustomerStandardPrivateSubscriptionProduct.SubscriptionName} for operator {CustomerStandardPrivateSubscriptionProduct.OperatorName} as standard private subscription product";
        }
    }
}
