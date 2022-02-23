namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class NewOperatorAccount
    {
        /// <summary>
        /// The number of the account used at the operator.
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// The name of the account used at the operator.
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// The operator identification.
        /// </summary>
        public int OperatorId { get; set; }
    }
}
