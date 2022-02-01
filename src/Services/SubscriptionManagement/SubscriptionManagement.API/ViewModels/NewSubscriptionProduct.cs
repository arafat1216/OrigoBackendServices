namespace SubscriptionManagement.API.ViewModels
{
    public record NewSubscriptionProduct
    {
        /// <summary>
        /// The product name
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// The operator name
        /// </summary>
        public string OperatorName { get; set; }
        /// <summary>
        /// The datapacakges that is offered by the product
        /// </summary>
        public IList<string>? DataPackages { get; set; }
    }
}
