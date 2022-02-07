namespace SubscriptionManagementServices.Models
{
    public class TransferSubscriptionOrder : SubscriptionOrder
    {
        public virtual CustomerOperatorAccount NewOperatorAccount { get; set; }
        public int NewOperatorAccountId { get; set; }
    }
}
