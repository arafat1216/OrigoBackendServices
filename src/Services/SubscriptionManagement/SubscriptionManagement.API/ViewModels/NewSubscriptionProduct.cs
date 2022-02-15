namespace SubscriptionManagement.API.ViewModels
{
    public record NewSubscriptionProduct
    {
        /// <summary>
        /// The product name
        /// </summary>
        public string SubscriptionProductName { get; set; }
        /// <summary>
        /// The operator name
        /// </summary>
        public string OperatorName { get; set; }
        /// <summary>
        /// The datapacakges that is offered by the product
        /// </summary>
        public IList<string>? DataPackages { get; set; }
        /// <summary>
        /// If the subscription product is global (true) or custom made (false)
        /// </summary>
        public bool IsGlobal { get; set; }
        /// <summary>
        /// Id of caller to identify who requested the action to be made
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
