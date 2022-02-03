namespace SubscriptionManagement.API.ViewModels
{
    public class SubscriptionOrder
    {
        public SubscriptionOrder(SubscriptionManagementServices.Models.SubscriptionOrder subscriptionOrder)
        {
            SubscriptionProductId = subscriptionOrder.SubscriptionProductId;
            OperatorAccountId = subscriptionOrder.OperatorAccountId;
            DatapackageId = subscriptionOrder.DatapackageId;
        }
        /// <summary>
        /// Subscription product identifier
        /// </summary>
        public int SubscriptionProductId { get; set; }
        /// <summary>
        /// Operator account identifier
        /// </summary>
        public int OperatorAccountId { get; set; }
        /// <summary>
        /// Datapackage identifier
        /// </summary>
        public int DatapackageId { get; set; }
        public Guid CallerId { get; set; }
    }
}
