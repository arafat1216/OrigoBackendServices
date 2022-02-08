namespace SubscriptionManagement.API.ViewModels
{
    public class CustomerOperatorAccount
    {
        public CustomerOperatorAccount()
        {

        }
        public CustomerOperatorAccount(SubscriptionManagementServices.Models.CustomerOperatorAccount customerOperatorAccount)
        {
            OrganizationId = customerOperatorAccount.OrganizationId;
            AccountNumber = customerOperatorAccount.AccountNumber;
            AccountName = customerOperatorAccount.AccountName;
            OrganizationId = customerOperatorAccount.OrganizationId;
            OperatorId = customerOperatorAccount.OperatorId;
        }

        /// <summary>
        /// Organization identifier
        /// </summary>
        public Guid OrganizationId { get; set; }
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
