using Common.Seedwork;
using SubscriptionManagementServices.DomainEvents;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubscriptionManagementServices.Models
{
    public class ChangeSubscriptionOrder : Entity, ISubscriptionOrder
    {

        public ChangeSubscriptionOrder()
        {
        }

        public ChangeSubscriptionOrder(string mobileNumber, string productName, string? packageName, string operatorName, string? subscriptionOwner, Guid organizationId, Guid callerId)
        {
            MobileNumber = mobileNumber;
            ProductName = productName;
            PackageName = packageName;
            OperatorName = operatorName;
            SubscriptionOwner = subscriptionOwner;
            OrganizationId = organizationId;
            CreatedBy = callerId;
            SubscriptionOrderId = Guid.NewGuid();
            AddDomainEvent(new ChangeSubscriptionOrderCreatedDomainEvent(this, callerId));
        }

        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string ProductName { get; set; }
        public string? PackageName { get; set; }
        public string? SubscriptionOwner { get; set; }
        public Guid OrganizationId { get; set; }


        #region ISubscriptionOrder Implementation

        [NotMapped] public string OrderType => "ChangeSubscription";

        [NotMapped] public string PhoneNumber => MobileNumber;

        [NotMapped] public string NewSubscriptionOrderOwnerName => SubscriptionOwner != null ? SubscriptionOwner : "Owner not specified";

        [NotMapped] public DateTime TransferDate => DateTime.UtcNow;

        public Guid SubscriptionOrderId { get; set; }
        public string? SalesforceTicketId { get; set; }
        #endregion
    }
}
