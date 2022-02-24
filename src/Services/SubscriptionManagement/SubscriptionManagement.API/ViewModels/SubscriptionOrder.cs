namespace SubscriptionManagement.API.ViewModels
{
    public class SubscriptionOrder
    {
        public SubscriptionOrder()
        {

        }
        public SubscriptionOrder(SubscriptionManagementServices.Models.SubscriptionOrder subscriptionOrder)
        {
            SubscriptionProductId = subscriptionOrder.SubscriptionProductId;
            OperatorAccountId = subscriptionOrder.OperatorAccountId;
            DataPackageId = subscriptionOrder.DataPackageId;
            OrganizationId = subscriptionOrder.OrganizationId;
            OrderExecutionDate = subscriptionOrder.OrderExecutionDate;
            SimCardNumber = subscriptionOrder.SimCardNumber;
        }
        /// <summary>
        /// Subscription product identifier
        /// </summary>
        public int SubscriptionProductId { get; set; }
        /// <summary>
        /// Current operator account identifier
        /// </summary>
        public int? OperatorAccountId { get; set; }
        /// <summary>
        /// DataPackage identifier
        /// </summary>
        public int DataPackageId { get; set; }
        /// <summary>
        /// Customer identifier
        /// </summary>
        public Guid OrganizationId { get; set; }
        public Guid CallerId { get; set; }
        /// <summary>
        /// SIM card number
        /// </summary>
        public string SimCardNumber { get; set; }
        /// <summary>
        /// Date of transfer
        /// </summary>
        public DateTime OrderExecutionDate { get; set; }
    }
}
