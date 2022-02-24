using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerSubscriptionProductdRemovedDomainEvent : BaseEvent
    {
        public CustomerSubscriptionProductdRemovedDomainEvent(CustomerSubscriptionProduct customerSubscriptionProduct, Guid customerId) : base(customerId)
        {
            CustomerSubscriptionProduct = customerSubscriptionProduct;
            CustomerId = customerId;
        }

        public CustomerSubscriptionProduct CustomerSubscriptionProduct { get; set; }
        public Guid CustomerId { get; set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Customer subscription product {CustomerSubscriptionProduct.SubscriptionName} was removed for customer id {CustomerId}";
        }
    }
}
