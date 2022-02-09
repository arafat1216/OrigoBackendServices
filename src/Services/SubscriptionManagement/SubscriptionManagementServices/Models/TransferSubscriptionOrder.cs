namespace SubscriptionManagementServices.Models
{
    public class TransferSubscriptionOrder : SubscriptionOrder
    {
        public TransferSubscriptionOrder() : base()
        {

        }
        public TransferSubscriptionOrder(Guid customerId, int subscriptionProductId, int currentOperatorAccountId, int datapackageId, Guid callerId, string simCardNumber, DateTime orderExecutionDate, int newOperatorAccountId)
            : base(customerId, subscriptionProductId, currentOperatorAccountId, datapackageId, callerId, simCardNumber)
        {
            OrderExecutionDate = orderExecutionDate;
            NewOperatorAccountId = newOperatorAccountId;
        }
        public virtual CustomerOperatorAccount NewOperatorAccount { get; set; }
        public int NewOperatorAccountId { get; set; }
    }
}
