using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerStandardPrivateSubscriptionProductAddedDomainEvent : BaseEvent
    {
        public CustomerStandardPrivateSubscriptionProductAddedDomainEvent(Guid organizationId, string operatorName,CustomerStandardPrivateSubscriptionProduct customerStandardPrivateSubscriptionProduct, Guid callerId) : base(organizationId)
        {
            CustomerStandardPrivateSubscriptionProduct = customerStandardPrivateSubscriptionProduct;
            CallerId = callerId;
            OrganizationId = organizationId;
            OperatorName = operatorName;
        }

        public CustomerStandardPrivateSubscriptionProduct CustomerStandardPrivateSubscriptionProduct { get; set; }
        public Guid OrganizationId { get; set; }
        public string OperatorName { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage()
        {
            return $"Customer id: {OrganizationId} added {CustomerStandardPrivateSubscriptionProduct.SubscriptionName} for operator {OperatorName} as standard private subscription product";
        }
    }
}
