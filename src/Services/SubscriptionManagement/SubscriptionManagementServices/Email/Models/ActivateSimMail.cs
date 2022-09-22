
namespace SubscriptionManagementServices.Email.Models
{
    public class ActivateSimMail
    {
        /// <summary>
        /// What type of order this is. 
        /// </summary>
        public string OrderType = "Activate SIM";
        /// <summary>
        /// The mobile number connected to the activated sim.
        /// </summary>
        public string MobileNumber { get; set; } = "N/A";
        /// <summary>
        /// Operator for the sim
        /// </summary>
        public string OperatorName { get; set; } = "N/A";
        /// <summary>
        /// Sim card number.
        /// </summary>
        public string SimCardNumber { get; set; } = "N/A";
        /// <summary>
        /// Type of simcard (micro-sim, nano-sim etc.)
        /// </summary>
        public string SimCardType { get; set; } = "N/A";
        /// <summary>
        /// Name of the resource file.
        /// </summary>
        public const string TemplateName = "ActivateSim";
        /// <summary>
        /// Subscription order id to have a unique identification of the subscription order.
        /// </summary>
        public string SubscriptionOrderId { get; set; } = "N/A";
    }
}
