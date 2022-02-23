namespace SubscriptionManagement.API.ViewModels
{
    public class CustomerOperatorAccount
    {
        public CustomerOperatorAccount()
        {

        }
        public CustomerOperatorAccount(SubscriptionManagementServices.ServiceModels.CustomerOperatorAccountDTO customerOperatorAccount)
        {
            Id = customerOperatorAccount.Id;
            AccountNumber = customerOperatorAccount.AccountNumber;
            AccountName = customerOperatorAccount.AccountName;
            OperatorId = customerOperatorAccount.OperatorId;
        }

        /// <summary>
        /// Account identifier
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Account number of the operator
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// Account name of the operator
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// Operator identifier
        /// </summary>
        public int OperatorId { get; set; }
        public Guid CallerId { get; set; }
    }
}
