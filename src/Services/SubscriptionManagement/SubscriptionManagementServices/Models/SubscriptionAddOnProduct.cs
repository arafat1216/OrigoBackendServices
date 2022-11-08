using Common.Seedwork;
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionAddOnProduct : Entity
    {
        public SubscriptionAddOnProduct()
        {
        }

        public SubscriptionAddOnProduct(string addOnProductName, Guid callerId)
        {
            AddOnProductName = addOnProductName;
            UpdatedBy = callerId;
            CreatedBy = callerId;
        }
        [MaxLength(50)]
        public string AddOnProductName { get; set; }

        public IReadOnlyCollection<TransferToBusinessSubscriptionOrder>? TransferToBusinessSubscriptionOrders { get; set; }
        public IReadOnlyCollection<NewSubscriptionOrder>? NewSubscriptionOrders { get; set; }
        public IReadOnlyCollection<CustomerStandardBusinessSubscriptionProduct> CustomerStandardBusinessSubscriptionProduct { get; set; }
    }
}
