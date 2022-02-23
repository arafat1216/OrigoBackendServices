namespace SubscriptionManagementServices.Models
{
    public class PrivateToBusinessSubscriptionOrder : SubscriptionOrder
    {
        public PrivateToBusinessSubscriptionOrder() : base()
        {

        }
        public PrivateToBusinessSubscriptionOrder(Guid customerId, int subscriptionProductId, int currentOperatorAccountId, int dataPackageId, Guid callerId, string simCardNumber, DateTime orderExecutionDate, int newOperatorAccountId)
            : base(customerId, subscriptionProductId, currentOperatorAccountId, dataPackageId, callerId, simCardNumber)
        {
            OrderExecutionDate = orderExecutionDate;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PostalPlace { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string OperatorName { get; set; }
    }
}
