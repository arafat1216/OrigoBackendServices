namespace SubscriptionManagement.API.ViewModels
{
    public record SubscriptionProductViewModel : NewSubscriptionProduct
    {
        public SubscriptionProductViewModel(SubscriptionManagementServices.Models.SubscriptionProduct subscriptionProduct)
        {

            ProductName = subscriptionProduct.SubscriptionName;
            OperatorName = subscriptionProduct.OperatorType.OperatorName;
            DataPackages = subscriptionProduct.DataPackages != null ? subscriptionProduct.DataPackages.Select(i => i.DatapackageName).ToList() : new List<string>();
        }
    }
}
