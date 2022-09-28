
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Email.Models
{
    public class OrderSimMail
    {
        /// <summary>
        /// What type of order this is. 
        /// </summary>
        public string OrderType { get; } = "Order Sim";
        /// <summary>
        /// Name of the resource file
        /// </summary>
        public const string TemplateName = "OrderSim";
        /// <summary>
        /// The order id in salesforce.
        /// </summary>
        public string SubscriptionOrderId { get; set; } = "N/A";
        /// <summary>
        /// The name of the person reciving the sim card(s).
        /// </summary>
        public string SendToName { get; set; } = "N/A";
        /// <summary>
        /// The operator of the sim card.
        /// </summary>
        public string OperatorName { get; set; } = "N/A";
        /// <summary>
        /// Amount of sim cards ordered.
        /// </summary>
        public string Quantity { get; set; } = "N/A";
        /// <summary>
        /// Street name that the sim card(s) should be sent.
        /// </summary>
        public string Street { get; set; } = "N/A";
        /// <summary>
        /// Post code that the sim card(s) should be sent.
        /// </summary>
        public string Postcode { get; set; } = "N/A";
        /// <summary>
        /// City that the sim card(s) should be sent.
        /// </summary>
        public string City { get; set; } = "N/A";
        /// <summary>
        /// Country that the sim card(s) should be sent.
        /// </summary>
        public string Country { get; set; } = "N/A";

    }
}
