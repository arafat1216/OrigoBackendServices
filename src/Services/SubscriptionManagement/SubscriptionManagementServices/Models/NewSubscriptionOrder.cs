using Common.Seedwork;
using SubscriptionManagementServices.DomainEvents;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
                                    string? operatorAccountNumber,
                                    string? operatorAccountName,
                                    string? operatorAccountOwner,
                                    string? operatorAccountPayer,
                                    string? operatorAccountOrganizationNumber,
                                    string subscriptionProductName,
                                    string? dataPackageName,
                                    DateTime orderExecutionDate,
                                    string? simCardNumber, string simCardAction,
                                    SimCardAddress? simCardAddress,
                                    string customerReferenceFields,
                                    List<SubscriptionAddOnProduct> subscriptionAddOnProducts,
                                    PrivateSubscription? privateSubscription,
                                    string? salesforceTicketId,
                                    Guid callerId)
        {
            OrganizationId = organizationId;
            MobileNumber = mobileNumber;
            OperatorId = operatorId;
            OperatorAccountNumber = operatorAccountNumber;
            OperatorAccountName = operatorAccountName;
            OperatorAccountOwner = operatorAccountOwner;
            OperatorAccountPayer = operatorAccountPayer;
            OperatorAccountOrganizationNumber = operatorAccountOrganizationNumber;
            SubscriptionProductName = subscriptionProductName;
            DataPackageName = dataPackageName;
            OrderExecutionDate = orderExecutionDate;
            SimCardNumber = simCardNumber;
            SimCardAction = simCardAction;
            SimCardAddress = simCardAddress;
            CustomerReferenceFields = customerReferenceFields;
            _subscriptionAddOnProducts = subscriptionAddOnProducts;
            PrivateSubscription = privateSubscription;
            SubscriptionOrderId = Guid.NewGuid();
            SalesforceTicketId = salesforceTicketId;
            CreatedBy = callerId;
            AddDomainEvent(new NewSubscriptionOrderCreatedDomainEvent(this, callerId));
        }


        public Guid OrganizationId { get; set; }
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string? OperatorAccountNumber { get; set; }
        public string? OperatorAccountName { get; set; }
        public string? OperatorAccountOwner { get; set; }
        public string? OperatorAccountPayer { get; set; }
        public string? OperatorAccountOrganizationNumber { get; set; }
        public string SubscriptionProductName { get; set; }
        public string? DataPackageName { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        [StringLength(22)]
        public string? SimCardNumber { get; set; }
        public string SimCardAction { get; set; }
        public SimCardAddress? SimCardAddress { get; set; }
        public PrivateSubscription? PrivateSubscription { get; set; }
        public string CustomerReferenceFields { get; set; }
        private readonly List<SubscriptionAddOnProduct> _subscriptionAddOnProducts;
        public IReadOnlyCollection<SubscriptionAddOnProduct> SubscriptionAddOnProducts => _subscriptionAddOnProducts.AsReadOnly();

        

        #region ISubscriptionOrder Implementation
        public Guid SubscriptionOrderId { get; set; }
        [NotMapped] public string OrderType => "NewSubscription";
        [NotMapped] public string PhoneNumber => MobileNumber;

        [NotMapped]
        public string NewSubscriptionOrderOwnerName =>
            PrivateSubscription != null ? $"{PrivateSubscription?.FirstName} {PrivateSubscription?.LastName}" : $"{SimCardAddress?.FirstName} {SimCardAddress?.LastName}" ?? string.Empty;

        [NotMapped] public DateTime ExecutionDate => OrderExecutionDate;
        public string? SalesforceTicketId { get; set; }
        #endregion
    }
}

