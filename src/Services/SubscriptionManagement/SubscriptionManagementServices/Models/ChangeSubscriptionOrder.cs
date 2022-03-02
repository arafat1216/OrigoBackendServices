using Common.Seedwork;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubscriptionManagementServices.Models
{
    public class ChangeSubscriptionOrder : Entity, ISubscriptionOrder
    {

        public ChangeSubscriptionOrder()
        {
        }

        public ChangeSubscriptionOrder(string mobileNumber, string productName, string packageName, string operatorName, DateTime orderExecutionDate, string orderAuthor, Guid organizationId)
        {
            MobileNumber = mobileNumber;
            ProductName = productName;
            PackageName = packageName;
            OperatorName = operatorName;
            OrderExecutionDate = orderExecutionDate;
            OrderAuthor = orderAuthor;
            OrganizationId = organizationId;
        }

        public string MobileNumber { get; set; }
        public string ProductName { get; set; }
        public string PackageName { get; set; }
        public string OperatorName { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public string OrderAuthor { get; set; }
        public Guid OrganizationId { get; set; }


        #region ISubscriptionOrder Implementation

        [NotMapped] public string OrderType => "ChangeSubscription";

        [NotMapped] public string PhoneNumber => MobileNumber;

        [NotMapped] public string NewSubscriptionOrderOwnerName => OrderAuthor;

        [NotMapped] public DateTime TransferDate => OrderExecutionDate;
        #endregion
    }
}
