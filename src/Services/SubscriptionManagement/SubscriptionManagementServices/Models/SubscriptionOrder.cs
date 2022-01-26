using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionOrder : Entity
    {
        public SubscriptionOrder(SubscriptionProduct subscriptionType, Guid organizationId, OperatorAccount operatorAccount, Datapackage? dataPackage, IList<SubscriptionAddOnProduct> subscriptionAddOnProducts)
        {
            SubscriptionType = subscriptionType;
            OrganizationId = organizationId;
            OperatorAccount = operatorAccount;
            DataPackage = dataPackage;
            SubscriptionAddOnProducts = subscriptionAddOnProducts;
        }

        public SubscriptionProduct SubscriptionType { get; set; }
        [Required]
        public Guid OrganizationId { get; set; }
        public OperatorAccount OperatorAccount { get; set; }
        public Datapackage? DataPackage { get; set; } 
        public IList<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; set; }
    }
}
