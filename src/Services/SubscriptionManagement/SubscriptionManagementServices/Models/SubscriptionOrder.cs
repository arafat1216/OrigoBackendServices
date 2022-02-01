using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionOrder : Entity
    {
        public SubscriptionOrder()
        {

        }
        public SubscriptionOrder(SubscriptionProduct subscriptionType, Guid organizationId, OperatorAccount operatorAccount, Datapackage? dataPackage, IList<SubscriptionAddOnProduct> subscriptionAddOnProducts)
        {
            SubscriptionType = subscriptionType;
            OrganizationId = organizationId;
            OperatorAccount = operatorAccount;
            DataPackage = dataPackage;
            SubscriptionAddOnProducts = subscriptionAddOnProducts;
        }
        
        public virtual SubscriptionProduct SubscriptionType { get; set; }
        public int SubscriptionProductId { get; set; }

        [Required]
        public Guid OrganizationId { get; set; }
        public virtual OperatorAccount OperatorAccount { get; set; }
        public int OperatorAccountId { get; set; }
        public virtual Datapackage? DataPackage { get; set; }
        public int DatapackageId { get; set; }
        public virtual ICollection<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; set; }
    }
}
