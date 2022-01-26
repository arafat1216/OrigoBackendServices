using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionAddOnProduct : Entity
    {
        public SubscriptionAddOnProduct(string addOnProductName)
        {
            AddOnProductName = addOnProductName;
        }
        public string AddOnProductName { get; set; }

    }
}
