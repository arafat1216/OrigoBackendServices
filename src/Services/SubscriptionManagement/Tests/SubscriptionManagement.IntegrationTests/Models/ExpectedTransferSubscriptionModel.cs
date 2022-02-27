using System;
using System.Collections.Generic;

namespace SubscriptionManagement.IntegrationTests.Models
{
    public record ExpectedTransferSubscriptionModel
    {
        /// <summary>
        /// The current owner the subscription will be transferred from.
        /// </summary>
        public ExpectedPrivateSubscription? PrivateSubscription { get; set; } = null;
        public ExpectedBusinessSubscription? BusinessSubscription { get; set; } = null;
        /// <summary>
        /// The mobile number to be transferred
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        ///     New operator account identifier
        /// </summary>
        public int? OperatorAccountId { get; set; }

        /// <summary>
        ///     Subscription product identifier
        /// </summary>
        public int SubscriptionProductId { get; set; }

        /// <summary>
        ///     Data package name
        /// </summary>
        public string DataPackage { get; set; }

        /// <summary>
        ///     SIM card number
        /// </summary>
        public string SIMCardNumber { get; set; }

        /// <summary>
        ///     SIM card number
        /// </summary>
        public string SIMCardAction { get; set; }

        /// <summary>
        ///     Date of transfer
        /// </summary>
        public DateTime OrderExecutionDate { get; set; }

        /// <summary>
        /// List of add on products to the subscription
        /// </summary>
        public IList<string> AddOnProducts { get; set; } = new List<string>();

        public IList<ExpectedNewCustomerReferenceField> CustomerReferenceFields { get; set; }
        public ExpectedNewOperatorAccountRequestedDTO? NewOperatorAccount { get; set; }
        public Guid CallerId { get; set; }
    }

    public record ExpectedPrivateSubscription
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalPlace { get; set; }
        public string? Country { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? OperatorName { get; set; }
        public ExpectedPrivateSubscription? RealOwner { get; set; }
    }

    public record ExpectedBusinessSubscription
    {
        public string? Name { get; set; }
        public string? OrganizationNumber { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalPlace { get; set; }
        public string? Country { get; set; }
    }

    public record ExpectedNewCustomerReferenceField
    {
        /// <summary>
        /// The name of the field used as a customer reference field.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of the reference field. Can be User or Account.
        /// </summary>
        public string Type { get; set; }
        public Guid CallerId { get; set; }
    }

    public record ExpectedNewOperatorAccountRequestedDTO
    {
        public string? NewOperatorAccountOwner { get; set; }
        public string? NewOperatorAccountPayer { get; set; }
    }
}
