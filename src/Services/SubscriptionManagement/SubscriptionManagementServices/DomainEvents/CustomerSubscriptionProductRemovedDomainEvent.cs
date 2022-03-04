using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents;

public class CustomerSubscriptionProductRemovedDomainEvent : BaseEvent
{
    public CustomerSubscriptionProductRemovedDomainEvent(CustomerSubscriptionProduct customerSubscriptionProduct,
        Guid customerId) : base(customerId)
    {
        // Create a partial deep copy.
        CustomerSubscriptionProduct = new CustomerSubscriptionProduct()
        {
            SubscriptionName = customerSubscriptionProduct.SubscriptionName,
            GlobalSubscriptionProduct = new SubscriptionProduct
            {
                OperatorId = customerSubscriptionProduct?.GlobalSubscriptionProduct?.OperatorId ?? 0,
                SubscriptionName = customerSubscriptionProduct?.GlobalSubscriptionProduct?.SubscriptionName ??
                                   string.Empty
            },
            Operator = new Operator
            {
                OperatorName = customerSubscriptionProduct?.Operator?.OperatorName ?? string.Empty
            }
        };
        CustomerId = customerId;
    }

    public CustomerSubscriptionProduct CustomerSubscriptionProduct { get; set; }
    public Guid CustomerId { get; set; }

    public override string EventMessage(string languageCode = "nb-NO")
    {
        return
            $"Customer subscription product {CustomerSubscriptionProduct.SubscriptionName} was removed for customer id {CustomerId}";
    }
}