using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Seedwork;
using SubscriptionManagementServices.Types;

namespace SubscriptionManagementServices.Models
{
    public class TransferToBusinessSubscriptionOrder : Entity, ISubscriptionOrder
    {
        private readonly List<SubscriptionAddOnProduct> _subscriptionAddOnProducts;

        public TransferToBusinessSubscriptionOrder()
        {
            _subscriptionAddOnProducts = new List<SubscriptionAddOnProduct>();
        }

        public TransferToBusinessSubscriptionOrder(string? simCardNumber, string simCardAction,
            CustomerSubscriptionProduct subscriptionProduct, Guid organizationId,
            CustomerOperatorAccount? customerOperatorAccount, string? operatorAccountNumber, string? dataPackageName, DateTime orderExecutionDate,
            string mobileNumber, string customerReferenceFields,
            List<SubscriptionAddOnProduct> subscriptionAddOnProducts, string? newOperatorAccountOwner, string? organizationNumberOwner,
            string? newOperatorAccountPayer, string? organizationNumberPayer,PrivateSubscription? privateSubscription,
            BusinessSubscription? businessSubscription, string newOperatorName, Guid callerId)
        {
            SubscriptionOrderId = Guid.NewGuid();
            SimCardNumber = simCardNumber;
            SimCardAction = simCardAction;
            SubscriptionProductName = subscriptionProduct.SubscriptionName;
            OrganizationId = organizationId;
            if (customerOperatorAccount != null)
            {
                OperatorAccountName = customerOperatorAccount.AccountName;
                OperatorName = customerOperatorAccount.Operator.OperatorName;
                OperatorAccountNumber = customerOperatorAccount.AccountNumber;
                OperatorAccountOrganizationNumber = customerOperatorAccount.ConnectedOrganizationNumber;
            }
            else
            {
                OperatorName = newOperatorName;
                OrganizationNumberPayer = organizationNumberPayer;
                OrganizationNumberOwner = organizationNumberOwner;
            }
            OperatorAccountPhoneNumber = operatorAccountNumber;
            DataPackageName = dataPackageName;
            OrderExecutionDate = orderExecutionDate;
            MobileNumber = mobileNumber;
            CustomerReferenceFields = customerReferenceFields;
            _subscriptionAddOnProducts = subscriptionAddOnProducts;
            OperatorAccountOwner = newOperatorAccountOwner;
            OperatorAccountPayer = newOperatorAccountPayer;
            PrivateSubscription = privateSubscription;
            BusinessSubscription = businessSubscription;
            CreatedBy = callerId;
        }

        public string SubscriptionProductName { get; set; }

        public string? OperatorAccountOrganizationNumber { get; set; }

        public string? OperatorAccountNumber { get; set; }
        public string? OperatorAccountPhoneNumber { get; set; }

        public string? OperatorName { get; set; }

        public string? OperatorAccountName { get; set; }

        public IReadOnlyCollection<SubscriptionAddOnProduct> SubscriptionAddOnProducts => _subscriptionAddOnProducts.AsReadOnly();

        [StringLength(22)]
        public string? SimCardNumber { get; set; }
        public string SimCardAction { get; set; }
        public Guid OrganizationId { get; set; }
        public string? DataPackageName { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerReferenceFields { get; set; }
        public string? OperatorAccountOwner { get; set; }
        public string? OrganizationNumberOwner { get; set; }
        public string? OperatorAccountPayer { get; set; }
        public string? OrganizationNumberPayer { get; set; }
        public PrivateSubscription? PrivateSubscription { get; set; }
        public BusinessSubscription? BusinessSubscription { get; set; }

        public void SetSubscriptionAddOnProduct(List<SubscriptionAddOnProduct> subscriptionAddOnProducts)
        {
            _subscriptionAddOnProducts.AddRange(subscriptionAddOnProducts);
        }

        #region ISubscriptionOrder implementation

        public Guid SubscriptionOrderId { get; set; }
        [NotMapped] public OrderTypes OrderType => OrderTypes.TransferToBusiness;
        [NotMapped] public string PhoneNumber => MobileNumber;

        [NotMapped]
        public string NewSubscriptionOrderOwnerName =>
            PrivateSubscription != null ? $"{PrivateSubscription?.FirstName} {PrivateSubscription?.LastName}" : BusinessSubscription?.Name ?? string.Empty;

        [NotMapped] public DateTime ExecutionDate => OrderExecutionDate;
        public string? SalesforceTicketId { get; set; }
        #endregion
    }
}
