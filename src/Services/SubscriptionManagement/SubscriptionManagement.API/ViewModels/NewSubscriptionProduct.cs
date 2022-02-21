namespace SubscriptionManagement.API.ViewModels
{
    public record NewSubscriptionProduct
    {
        /// <summary>
        /// The subscription product name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The operator name
        /// </summary>
        public int OperatorId { get; set; }
        /// <summary>
        /// The datapacakges that is offered by the product
        /// </summary>
        public IList<string>? DataPackages { get; set; }
        /// <summary>
        /// Id of caller to identify who requested the action to be made
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
