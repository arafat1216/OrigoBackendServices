using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerSubscriptionProductAddedDomainEvent : BaseEvent 
    {

        public CustomerSubscriptionProductAddedDomainEvent(Guid customerId, CustomerSubscriptionProduct customerSubscriptionProduct, Guid callerId) : base(customerId)
        {
            CustomerId = customerId;
            CustomerSubscriptionProduct = customerSubscriptionProduct;
            CallerId = callerId;
        }
        public Guid CustomerId { get; set; }
        public CustomerSubscriptionProduct CustomerSubscriptionProduct { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage(string languageCode = "nb-NO")
        {
            return $"Subscription {CustomerSubscriptionProduct.SubscriptionName}, for customer id {CustomerId} was added";
        }
    }
}
