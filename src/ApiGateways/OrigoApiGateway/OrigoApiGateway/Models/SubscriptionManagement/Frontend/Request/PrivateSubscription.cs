#nullable enable

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class PrivateSubscription
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }

        public string? PostalCode { get; set; }

        public string? PostalPlace { get; set; }

        // TODO: Should this receive the min/max attribute so it correctly enforces the 2-character ISO code?
        [MaxLength(2)]
        public string? Country { get; set; }

        [EmailAddress]
        [MaxLength(320)]
        public string? Email { get; set; }

        public DateTime? BirthDate { get; set; }
        [MaxLength(50)]
        public string? OperatorName { get; set; }

        public PrivateSubscription? RealOwner { get; set; }
    }
}