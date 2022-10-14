using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Seedwork;
using SubscriptionManagementServices.Types;

namespace SubscriptionManagementServices.Models
{
    public class TransferToPrivateSubscriptionOrder : Entity, ISubscriptionOrder
    {
        public TransferToPrivateSubscriptionOrder()
        {

        }

        public PrivateSubscription UserInfo { get; set; }
        [MaxLength(15)]
        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string NewSubscription { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public Guid OrganizationId { get; set; }

        #region ISubscriptionOrder implementation

        public Guid SubscriptionOrderId { get; set; } = Guid.NewGuid();
        [NotMapped] public OrderTypes OrderType => OrderTypes.TransferToPrivate;
        [NotMapped] public string PhoneNumber => MobileNumber;

        [NotMapped]
        public string NewSubscriptionOrderOwnerName => $"{UserInfo?.FirstName} {UserInfo?.LastName}";
        [NotMapped] public DateTime ExecutionDate => OrderExecutionDate;
        public string? SalesforceTicketId { get; set; }
        #endregion
    }
}