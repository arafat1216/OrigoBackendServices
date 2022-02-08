using Common.Seedwork;

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

        public string AddOnProductName { get; set; }
    }
}
