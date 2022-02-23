namespace SubscriptionManagementServices.Models
{
    public class PrivateToBusinessSubscriptionOrder : SubscriptionOrder
    {
        public PrivateToBusinessSubscriptionOrder() : base()
        {

        }
        public PrivateToBusinessSubscriptionOrder(
            Guid customerId, 
            int subscriptionProductId, 
            string currentOperatorName, 
            int dataPackageId, 
            Guid callerId, 
            string simCardNumber, 
            DateTime orderExecutionDate, 
            int operatorAccountId)
            : base(customerId, subscriptionProductId, operatorAccountId, dataPackageId, callerId, simCardNumber)
        {
            OrderExecutionDate = orderExecutionDate;
            OperatorName = currentOperatorName;
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
