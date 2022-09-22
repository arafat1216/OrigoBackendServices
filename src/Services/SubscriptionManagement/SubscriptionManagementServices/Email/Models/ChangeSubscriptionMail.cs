
namespace SubscriptionManagementServices.Email.Models
{
    public class ChangeSubscriptionMail
    {
        /// <summary>
        /// What type of order this is. 
        /// </summary>
        public string OrderType = "Change subscription";
        /// <summary>
        /// Name of the resource file
        /// </summary>
        public const string TemplateName = "ChangeSubscription";
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
        /// The name of the new subscription product.
        /// </summary>
        public string ProductName { get; set; } = "N/A";
        /// <summary>
        /// Package that is ordered with the subscription product (data packages etc.)
        /// </summary>
        public string PackageName { get; set; } = "N/A";
    }
}
