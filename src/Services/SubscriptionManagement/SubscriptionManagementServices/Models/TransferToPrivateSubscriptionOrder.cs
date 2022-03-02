using System.ComponentModel.DataAnnotations.Schema;
using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class TransferToPrivateSubscriptionOrder : Entity, ISubscriptionOrder
    {
        public TransferToPrivateSubscriptionOrder()
        {

        }

        public TransferToPrivateSubscriptionOrder(PrivateSubscription userInfo, string operatorName, string newSubscription, DateTime orderExecutionDate, string mobileNumber)
        {
            UserInfo = userInfo;
            OperatorName = operatorName;
            NewSubscription = newSubscription;
            OrderExecutionDate = orderExecutionDate;
            MobileNumber = mobileNumber;
        }

        public PrivateSubscription UserInfo { get; set; }
        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string NewSubscription { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public Guid OrganizationId { get; set; }

        #region ISubscriptionOrder implementation
        [NotMapped] public string OrderType => "TransferToPrivate";
        [NotMapped] public string PhoneNumber => MobileNumber;

        [NotMapped]
        public string NewSubscriptionOrderOwnerName => $"{UserInfo?.FirstName} {UserInfo?.LastName}";
        [NotMapped] public DateTime TransferDate => OrderExecutionDate;
        #endregion
    }
}