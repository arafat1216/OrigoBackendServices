
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Email.Models
{
    public class TransferToPrivateMail
    {
        /// <summary>
        /// What type of order this is. 
        /// </summary>
        public string OrderType = "Transfer to private";
        /// <summary>
        /// Information about the user that is transfering to a private subscription.
        /// </summary>
        public PrivateSubscriptionMail UserInfo { get; set; }
        /// <summary>
        /// Mobile number connected to the subscription.
        /// </summary>
        public string MobileNumber { get; set; } = "N/A";
        /// <summary>
        /// Operator connected to the subscription.
        /// </summary>
        public string OperatorName { get; set; } = "N/A";
        /// <summary>
        /// The new private subscription name that the user is changing to.
        /// </summary>
        public string NewSubscription { get; set; } = "N/A";
        /// <summary>
        /// The date the transfer sets in place.
        /// </summary>
        public string OrderExecutionDate { get; set; } = "N/A";
        /// <summary>
        /// Subscription order id.
        /// </summary>
        public string SubscriptionOrderId { get; set; } = "N/A";
        /// <summary>
        /// Name of the resource file
        /// </summary>
        public const string TemplateName = "TransferToPrivate";
    }
}
