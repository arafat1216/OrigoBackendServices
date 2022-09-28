
namespace SubscriptionManagementServices.Email.Models
{
    public class CancelSubscriptionMail
    {
        /// <summary>
        /// What type of order this is. 
        /// </summary>
        public string OrderType { get;  } = "Cancel subscription";
        /// <summary>
        /// Name of the resource file
        /// </summary>
        public const string TemplateName = "CancelSubscription";
        /// <summary>
        /// The order id in salesforce.
        /// </summary>
        public string SubscriptionOrderId { get; set; } = "N/A";
        /// <summary>
        /// Mobile number connected to the subscription.
        /// </summary>
        public string MobileNumber { get; set; } = "N/A";
        /// <summary>
        /// Operator connected to the subscription.
        /// </summary>
        public string OperatorName { get; set; } = "N/A";
        /// <summary>
        /// The date the subscription teminates.
        /// </summary>
        public string DateOfTermination { get; set; } = "N/A";
    }
}
