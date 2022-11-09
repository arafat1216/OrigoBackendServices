using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerStandardBusinessSubscriptionProductRemovedDomainEvent : BaseEvent
    {
        public CustomerStandardBusinessSubscriptionProductRemovedDomainEvent(CustomerStandardBusinessSubscriptionProduct standardBusinessProduct, int operatorId, Guid callerId, Guid organizationId) : base(organizationId)
        {
            StandardBusinessProduct = standardBusinessProduct;
            CallerId = callerId;
            OrganizationId = organizationId;
            OperatorId = operatorId;
        }

        public CustomerStandardBusinessSubscriptionProduct StandardBusinessProduct { get; set; }
        public Guid CallerId { get; set; }
        public Guid OrganizationId { get; set; }
        public int OperatorId { get; set; }

        public override string EventMessage()
        {
            return $"Customer with id: {OrganizationId} removed private standard subscription product {StandardBusinessProduct.SubscriptionName} with operator id {OperatorId}";
        }
    }
}
