using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class TransferToPrivateSubscriptionOrder : Entity
    {
        public TransferToPrivateSubscriptionOrder()
        {

        }

        public TransferToPrivateSubscriptionOrder(PrivateSubscription userInfo, string operatorName, string newSubscription, DateTime orderExecutionDate)
        {
            UserInfo = userInfo;
            OperatorName = operatorName;
            NewSubscription = newSubscription;
            OrderExecutionDate = orderExecutionDate;
        }

        public PrivateSubscription UserInfo { get; set; }
        public string OperatorName { get; set; }
        public string NewSubscription { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
