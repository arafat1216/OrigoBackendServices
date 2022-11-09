using Common.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.DomainEvents
{
    public class CustomerStandardBusinessSubscriptionProductAddedDomainEvent : BaseEvent
    {
        public CustomerStandardBusinessSubscriptionProductAddedDomainEvent(Guid organizationId, string operatorName, CustomerStandardBusinessSubscriptionProduct customerStandardBusinessSubscriptionProduct, Guid callerId) : base(organizationId)
        {
            CustomerStandardBusinessSubscriptionProduct = customerStandardBusinessSubscriptionProduct;
            CallerId = callerId;
            OrganizationId = organizationId;
            OperatorName = operatorName;
        }

        public CustomerStandardBusinessSubscriptionProduct CustomerStandardBusinessSubscriptionProduct { get; set; }
        public Guid OrganizationId { get; set; }
        public string OperatorName { get; set; }
        public Guid CallerId { get; set; }

        public override string EventMessage()
        {
            return $"Customer id: {OrganizationId} added {CustomerStandardBusinessSubscriptionProduct.SubscriptionName} for operator {OperatorName} as standard business subscription product";
        }
    }
}
