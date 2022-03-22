using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerStandardPrivateSubscriptionProductRemovedDomainEvent : BaseEvent
    {
        public CustomerStandardPrivateSubscriptionProductRemovedDomainEvent(CustomerStandardPrivateSubscriptionProduct standardPrivateProduct, int operatorId,Guid callerId, Guid organizationId) : base(organizationId)
        {
            StandardPrivateProduct = standardPrivateProduct;
            CallerId = callerId;
            OrganizationId = organizationId;
            OperatorId = operatorId;
        }

        public CustomerStandardPrivateSubscriptionProduct StandardPrivateProduct { get; set; }
        public Guid CallerId { get; set; }
        public Guid OrganizationId { get; set; }
        public int OperatorId { get; set; }

        public override string EventMessage()
        {
            return $"Customer with id: {OrganizationId} removed private standard subscription product {StandardPrivateProduct.SubscriptionName} with operator id {OperatorId}";
        }
    }
}
