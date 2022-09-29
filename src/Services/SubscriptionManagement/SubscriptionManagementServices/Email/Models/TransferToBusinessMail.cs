
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Email.Models
{
    public class TransferToBusinessMail
    {
        /// <summary>
        /// What type of order this is. 
        /// </summary>
        public string OrderType { get;  } = "Transfer to business subscription";
        /// <summary>
        /// Name of the resource file
        /// </summary>
        public const string TemplateName = "TransferToBusiness";
        /// <summary>
        /// The order id in salesforce.
        /// </summary>
        public string SubscriptionOrderId { get; set; } = "N/A";
        /// <summary>
        /// Information about the private user.
        /// </summary>
        public PrivateSubscriptionMail? PrivateSubscription { get; set; }
        /// <summary>
        /// Information about the business subscription.
        /// </summary>
        public BusinessSubscriptionMail? BusinessSubscription { get; set; }
        /// <summary>
        /// Mobilenumber connected to the subscription.
        /// </summary>
        public string MobileNumber { get; set; } = "N/A";
        /// <summary>
        /// Operator for the new subscription.
        /// </summary>
        public string OperatorName { get; set; } = "N/A";
        /// <summary>
        /// The subscription product name.
        /// </summary>
        public string SubscriptionProductName { get; set; } = "N/A";
        /// <summary>
        /// The data package that is connected to the subscription.
        /// </summary>
        public string? DataPackage { get; set; } = "N/A";
        /// <summary>
        /// Date the business subscription is set to start.
        /// </summary>
        public string OrderExecutionDate { get; set; } = "N/A";
        /// <summary>
        /// Userferferences.
        /// </summary>
        public string UserReferences { get; set; } = "N/A";
        /// <summary>
        /// The organizations account at the given operator.
        /// </summary>
        public string OperatorAccount { get; set; } = "N/A";
        /// <summary>
        /// Payers operator account.
        /// </summary>
        public string OperatorAccountPayer { get; set; } = "N/A";
        /// <summary>
        /// Owners operator account.
        /// </summary>
        public string OperatorAccountOwner { get; set; } = "N/A";
        /// <summary>
        /// Sim card number to be used when activating the sim.
        /// </summary>
        public string? SimCardNumber { get; set; } = "N/A";
        /// <summary>
        /// Which action the subscriber would like to do with the sim (e.g keep, new etc.)
        /// </summary>
        public string SimCardAction { get; set; } = "N/A";
        /// <summary>
        /// Where the new sim card should be sent.
        /// </summary>
        public SimCardAddress? SimCardAddress { get; set; }
        /// <summary>
        /// The owner of the subscription.
        /// </summary>
        public PrivateSubscriptionMail? RealOwner { get; set; }
        /// <summary>
        /// Existing operator account for the customer.
        /// </summary>
        public string? CustomersOperatorAccount { get; set; } = "N/A";
        /// <summary>
        /// Reference phone number to be used when filling in operator account.
        /// </summary>
        public string? OperatorAccountMobileNumber { get; set; } = "N/A";
        /// <summary>
        /// Add on products that the subscription is ordered with.
        /// </summary>
        public string SubscriptionAddOnProducts { get; set; } = "N/A";
    }
}
