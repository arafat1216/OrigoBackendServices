namespace SubscriptionManagementServices.Models
{
    public class TransferSubscriptionOrder : SubscriptionOrder
    {
        public TransferSubscriptionOrder() : base()
        {

        }
        public TransferSubscriptionOrder(Guid customerId, int subscriptionProductId, int currentOperatorAccountId, int dataPackageId, Guid callerId, string simCardNumber, DateTime orderExecutionDate, int newOperatorAccountId)
            : base(customerId, subscriptionProductId, currentOperatorAccountId, dataPackageId, callerId, simCardNumber)
        {
            OrderExecutionDate = orderExecutionDate;
            NewOperatorAccountId = newOperatorAccountId;
        }
        public virtual CustomerOperatorAccount NewOperatorAccount { get; set; }
        public int NewOperatorAccountId { get; set; }
    }
}
