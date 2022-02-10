namespace SubscriptionManagement.API.ViewModels
{
    public class TransferSubscriptionOrder : SubscriptionOrder
    {
        public TransferSubscriptionOrder() : base()
        {

        }
        public TransferSubscriptionOrder(SubscriptionManagementServices.Models.SubscriptionOrder subscriptionOrder) : base(subscriptionOrder)
        {

        }

        /// <summary>
        /// New operator account identifier
        /// </summary>
        public int NewOperatorAccountId { get; set; }
    }
}
