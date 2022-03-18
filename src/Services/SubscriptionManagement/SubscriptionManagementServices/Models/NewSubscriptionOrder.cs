using Common.Seedwork;
using SubscriptionManagementServices.DomainEvents;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SubscriptionManagementServices.Models
{
    public class NewSubscriptionOrder : Entity, ISubscriptionOrder
    {
        public NewSubscriptionOrder()
        {
        }

        public NewSubscriptionOrder(Guid organizationId,
                                    string mobileNumber,
                                    int operatorId,
                                    CustomerOperatorAccount? existingAccount,
                                    string? operatorAccountOwner,
                                    string? operatorAccountPayer,
                                    string subscriptionProductName,
                                    string? dataPackageName,
                                    DateTime orderExecutionDate,
                                    string? simCardNumber, 
                                    string simCardAction,
                                    SimCardAddress? simCardAddress,
                                    string customerReferenceFields,
                                    List<SubscriptionAddOnProduct>? subscriptionAddOnProducts,
                                    PrivateSubscription? privateSubscription,
                                    BusinessSubscription? businessSubscription,
                                    Guid callerId)
        {
            OrganizationId = organizationId;
            MobileNumber = mobileNumber;
            OperatorId = operatorId;
            if (existingAccount != null) 
            {
                OperatorAccountNumber = existingAccount.AccountNumber;
                OperatorAccountName = existingAccount.AccountName;
                OperatorAccountOrganizationNumber = existingAccount.ConnectedOrganizationNumber;
                OperatorId = existingAccount.OperatorId;
            }
            else
            {
                OperatorAccountPayer = operatorAccountPayer;
                OperatorAccountOwner = operatorAccountOwner;
            }
            SubscriptionProductName = subscriptionProductName;
            DataPackageName = dataPackageName;
            OrderExecutionDate = orderExecutionDate;
            SimCardNumber = simCardNumber;
            SimCardAction = simCardAction;
            if (simCardAddress != null)
            {
                SimCardReceiverAddress = simCardAddress.Address;
                SimCardReceiverFirstName = simCardAddress.FirstName;
                SimCardReceiverLastName = simCardAddress.LastName;
                SimCardReceiverCountry = simCardAddress.Country;
                SimCardReceiverPostalCode = simCardAddress.PostalCode;
                SimCardReceiverPostalPlace = simCardAddress.PostalPlace;
            }
            else
            {
                SimCardReceiverAddress = businessSubscription.Address;
                SimCardReceiverCountry= businessSubscription.Country;
                SimCardReceiverPostalCode= businessSubscription.PostalCode;
                SimCardReceiverPostalPlace= businessSubscription.PostalPlace;
                SimCardReceiverFirstName = businessSubscription.Name;
                SimCardReceiverLastName = businessSubscription.OrganizationNumber;
            }
            CustomerReferenceFields = customerReferenceFields;
            _subscriptionAddOnProducts = subscriptionAddOnProducts;
            PrivateSubscription = privateSubscription;
            BusinessSubscription = businessSubscription;
            SubscriptionOrderId = Guid.NewGuid();
            CreatedBy = callerId;
            AddDomainEvent(new NewSubscriptionOrderCreatedDomainEvent(this, callerId));
        }


        public Guid OrganizationId { get; set; }
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string SubscriptionProductName { get; set; }
        public string? DataPackageName { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public string? OperatorAccountNumber { get; set; }
        public string? OperatorAccountName { get; set; }
        public string? OperatorAccountOwner { get; set; }
        public string? OperatorAccountPayer { get; set; }
        public string? OperatorAccountOrganizationNumber { get; set; }
        public PrivateSubscription? PrivateSubscription { get; set; }
        public BusinessSubscription? BusinessSubscription { get; set; }
        [StringLength(22)]
        public string? SimCardNumber { get; set; }
        public string SimCardAction { get; set; }
        public string? SimCardReceiverFirstName { get; set; }
        public string? SimCardReceiverLastName { get; set; }
        public string? SimCardReceiverAddress { get; set; }
        public string? SimCardReceiverPostalCode { get; set; }
        public string? SimCardReceiverPostalPlace { get; set; }
        public string? SimCardReceiverCountry { get; set; }
        public string CustomerReferenceFields { get; set; }

        private readonly List<SubscriptionAddOnProduct> _subscriptionAddOnProducts;
        [JsonIgnore]
        public IReadOnlyCollection<SubscriptionAddOnProduct> SubscriptionAddOnProducts => _subscriptionAddOnProducts.AsReadOnly();

        

        #region ISubscriptionOrder Implementation
        public Guid SubscriptionOrderId { get; set; }
        [NotMapped] public string OrderType => "NewSubscription";
        [NotMapped] public string PhoneNumber => MobileNumber;

        [NotMapped]
        public string NewSubscriptionOrderOwnerName =>
            PrivateSubscription != null ? $"{PrivateSubscription?.FirstName} {PrivateSubscription?.LastName}" : $"{SimCardReceiverFirstName} {SimCardReceiverLastName}" ?? string.Empty;

        [NotMapped] public DateTime ExecutionDate => OrderExecutionDate;
        public string? SalesforceTicketId { get; set; }
        #endregion
    }
}

