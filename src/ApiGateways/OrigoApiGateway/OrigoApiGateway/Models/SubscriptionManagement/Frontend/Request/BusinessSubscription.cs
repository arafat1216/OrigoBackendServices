namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class BusinessSubscription
    {
        public string? Name { get; set; }
        public string? OrganizationNumber { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalPlace { get; set; }
        [MaxLength(2)]
        public string? Country { get; set; }
        public string? ContactPerson { get; set; }

    }
}
