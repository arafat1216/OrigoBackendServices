namespace SubscriptionManagementServices.ServiceModels
{
    public class NewCustomerReferenceField
    {
        /// <summary>
        /// The name of the field used as a customer reference field.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The type of the reference field. Can be User or Account.
        /// </summary>
        public string Type { get; set; }
        public Guid CallerId { get; set; }
    }
}
