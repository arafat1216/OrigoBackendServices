
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Email.Models
{
    public class NewSubscriptionMail
    {
        /// <summary>
        /// What type of order this is. 
        /// </summary>
        public string OrderType { get;  } = "New subscription";
        /// <summary>
        /// Name of the resource file that fetched from Resources.SubscriptionManagement.
        /// </summary>
        public const string TemplateName = "NewSubscription";
        /// <summary>
        /// The order id in salesforce.
        /// </summary>
        public string SubscriptionOrderId { get; set; } = "N/A";
        /// <summary>
        /// Information about the business user.
        /// </summary>
        public BusinessSubscriptionMail? BusinessSubscription { get; set; }
        /// <summary>
        /// Information about the private user.
        /// </summary>
        public PrivateSubscriptionMail? PrivateSubscription { get; set; }

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
        public string OrderExecutionDate { get; set; } = "N/A";
        /// <summary>
        /// Payer operator account.
        /// </summary>
        public string? OperatorAccountPayer { get; set; } = "N/A";
        /// <summary>
        /// Owner operator account.
        /// </summary>
        public string? OperatorAccountOwner { get; set; } = "N/A";
        /// <summary>
        /// The organizations account at the given operator.
        /// </summary>
        public string? OperatorAccountName { get; set; } = "N/A";
        /// <summary>
        /// Sim card number to be used when activating the sim.
        /// </summary>
        public string? SimCardNumber { get; set; } = "N/A";
        /// <summary>
        /// Which action the subscriber would like to do with the sim (e.g keep, new etc.)
        /// </summary>
        public string SimCardAction { get; set; } = "N/A";
        /// <summary>
        /// Which address the sim card should be sent to.
        /// </summary>
        public SimCardAddress? SimCardAddress { get; set; }
        /// <summary>
        /// The subscription product name.
        /// </summary>
        public string SubscriptionProductName { get; set; } = "N/A";
        /// <summary>
        /// Add on products that the subscription is ordered with.
        /// </summary>
        public string SubscriptionAddOnProducts { get; set; } = "N/A";
        /// <summary>
        /// Reference fields connecte to the operator. 
        /// </summary>
        public string CustomerReferenceFields { get; set; } = "N/A";
        /// <summary>
        /// Data package the subscription is ordered with.
        /// </summary>
        public string? DataPackage { get; set; } = "N/A";
        /// <summary>
        /// Existing operator account for the customer.
        /// </summary>
        public string? CustomersOperatorAccount { get; set; } = "N/A";
        /// <summary>
        /// Reference phone number to be used when filling in operator account.
        /// </summary>
        public string? OperatorAccountMobileNumber { get; set; } = "N/A";

    }
}
