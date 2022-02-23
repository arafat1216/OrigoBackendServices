namespace SubscriptionManagement.API.ViewModels
{
    public record NewOperatorAccount
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
        /// <summary>
        /// Identification for the caller that made the request.
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
