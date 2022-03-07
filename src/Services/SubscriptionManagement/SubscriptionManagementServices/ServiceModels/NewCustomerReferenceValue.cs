namespace SubscriptionManagementServices.ServiceModels
{
    public class NewCustomerReferenceValue
    {
        /// <summary>
        /// The name of the field used as a customer reference field.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of the reference field. Can be User or Account.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The value given for the customer reference field.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// The id of the caller.
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
