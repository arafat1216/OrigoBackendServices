using System;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public class OrigoCustomerOperatorAccount
    {
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
        /// <summary>
        /// Organization identifier associated with the account
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// Organization linked to the account.
        /// </summary>
        public string ConnectedOrganizationNumber { get; set; }
    }
}
