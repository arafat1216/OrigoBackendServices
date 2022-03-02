using System.ComponentModel.DataAnnotations.Schema;
using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class TransferToBusinessSubscriptionOrder : Entity, ISubscriptionOrder
    {
        private readonly List<SubscriptionAddOnProduct> _subscriptionAddOnProducts;

        public TransferToBusinessSubscriptionOrder()
        {

        }

        public TransferToBusinessSubscriptionOrder(
            string simCardNumber,
            string simCardAction,
            int subscriptionProductId,
            Guid organizationId,
            int? operatorAccountId,
            int dataPackageId,
            DateTime orderExecutionDate,
            string mobileNumber,
            string customerReferenceFields,
            List<SubscriptionAddOnProduct> subscriptionAddOnProducts,
            string? newOperatorAccountOwner,
            string? newOperatorAccountPayer,
            PrivateSubscription? privateSubscription,
            BusinessSubscription? businessSubscription)
        {
            SimCardNumber = simCardNumber;
            SIMCardAction = simCardAction;
            SubscriptionProductId = subscriptionProductId;
            OrganizationId = organizationId;
            OperatorAccountId = operatorAccountId;
            DataPackageId = dataPackageId;
            OrderExecutionDate = orderExecutionDate;
            MobileNumber = mobileNumber;
            CustomerReferenceFields = customerReferenceFields;
            _subscriptionAddOnProducts = subscriptionAddOnProducts;
            OperatorAccountOwner = newOperatorAccountOwner;
            OperatorAccountPayer = newOperatorAccountPayer;
            PrivateSubscription = privateSubscription;
            BusinessSubscription = businessSubscription;
        }
        public CustomerSubscriptionProduct CustomerSubscriptionProduct { get; set; }
        public IReadOnlyCollection<SubscriptionAddOnProduct> SubscriptionAddOnProducts => _subscriptionAddOnProducts.AsReadOnly();

        public string? SimCardNumber { get; set; }
        public string SIMCardAction { get; set; }
        public int SubscriptionProductId { get; set; }
        public Guid OrganizationId { get; set; }
        public CustomerOperatorAccount? OperatorAccount { get; set; }
        public int? OperatorAccountId { get; set; }
        public string? DataPackageName { get; set; }
        public int DataPackageId { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerReferenceFields { get; set; }

        public string? OperatorAccountOwner { get; set; }
        public string? OperatorAccountPayer { get; set; }

        public PrivateSubscription? PrivateSubscription { get; set; }
        public BusinessSubscription? BusinessSubscription { get; set; }

        public void SetSubscriptionAddOnProduct(List<SubscriptionAddOnProduct> subscriptionAddOnProducts)
        {
            _subscriptionAddOnProducts.AddRange(subscriptionAddOnProducts);
        }

        #region ISubscriptionOrder implementation
        [NotMapped] public string OrderType => "TransferToBusiness";
        [NotMapped] public string PhoneNumber => MobileNumber;

        [NotMapped]
        public string NewSubscriptionOrderOwnerName =>
            PrivateSubscription != null ? $"{PrivateSubscription?.FirstName} {PrivateSubscription?.LastName}" : BusinessSubscription?.Name ?? string.Empty;

        [NotMapped] public DateTime TransferDate => OrderExecutionDate;

        #endregion
    }
}
