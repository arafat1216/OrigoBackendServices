using System;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public class TransferSubscriptionOrder
    {
        /// <summary>
        /// New operator account identifier
        /// </summary>
        public int NewOperatorAccountId { get; set; }
        /// <summary>
        /// Subscription product identifier
        /// </summary>
        public int SubscriptionProductId { get; set; }
        /// <summary>
        /// Current operator account identifier
        /// </summary>
        public int OperatorAccountId { get; set; }
        /// <summary>
        /// Datapackage identifier
        /// </summary>
        public int DatapackageId { get; set; }
        /// <summary>
        /// Customer identifer
        /// </summary>
        public Guid CustomerId { get; set; }
        public Guid CallerId { get; set; }
        /// <summary>
        /// SIM card number
        /// </summary>
        public string SIMCardNumber { get; set; }
        /// <summary>
        /// Date of transfer
        /// </summary>
        public DateTime OrderExecutionDate { get; set; }
    }
}
