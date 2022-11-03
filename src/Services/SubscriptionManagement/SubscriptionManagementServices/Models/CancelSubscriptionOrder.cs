using Common.Enums;
using Common.Seedwork;
using SubscriptionManagementServices.DomainEvents;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubscriptionManagementServices.Models
{
    public class CancelSubscriptionOrder : Entity, ISubscriptionOrder
    {

        public CancelSubscriptionOrder()
        {
        }

        public CancelSubscriptionOrder(string mobileNumber, DateTime dateOfTermination, string operatorName, Guid organizationId, Guid callerId)
        {
            MobileNumber = mobileNumber;
            OperatorName = operatorName;
            DateOfTermination = dateOfTermination;
            OrganizationId = organizationId;
            SubscriptionOrderId = Guid.NewGuid();
            CreatedBy = callerId;
            AddDomainEvent(new CancelSubscriptionOrderCreatedDomainEvent(this, callerId));
        }
        public string MobileNumber { get; set; }
        public DateTime DateOfTermination { get; set; }
        public string OperatorName { get; set; }
        public Guid OrganizationId { get; set; }

        #region ISubscriptionOrder Implementation

        [NotMapped] public SubscriptionOrderTypes OrderType => SubscriptionOrderTypes.CancelSubscription;

        [NotMapped] public string PhoneNumber => MobileNumber;

        [NotMapped] public string NewSubscriptionOrderOwnerName => MobileNumber;

        [NotMapped] public DateTime OrderExecutionDate => DateOfTermination;

        public Guid SubscriptionOrderId { get; set; }
        public string? SalesforceTicketId { get; set; }
        #endregion
    }
}
