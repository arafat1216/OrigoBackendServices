namespace SubscriptionManagementServices.ServiceModels
{
    public record NewChangeSubscriptionOrder
    {
        public NewChangeSubscriptionOrder(string mobileNumber, string operatorName, string productName, string? packageName, string? subscriptionOwner, Guid callerId)
        {
            MobileNumber = mobileNumber;
            OperatorName = operatorName;
            ProductName = productName;
            PackageName = packageName;
            SubscriptionOwner = subscriptionOwner;
            CallerId = callerId;
        }

        public string MobileNumber { get; set;}
        public string OperatorName { get; set;}
        public string ProductName { get; set; }
        public string? PackageName { get; set; }
        public string? SubscriptionOwner { get; set; }
        public Guid CallerId { get; set; }
    }
}
