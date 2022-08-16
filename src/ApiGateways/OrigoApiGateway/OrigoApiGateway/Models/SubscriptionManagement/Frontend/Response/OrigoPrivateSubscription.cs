#nullable enable

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoPrivateSubscription
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Address { get; set; }

        public string? PostalCode { get; set; }

        public string? PostalPlace { get; set; }

        // TODO: Should this use the min/max length attribute to enforce the 2-character ISO code we are using?
        public string? Country { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public DateTime? BirthDate { get; set; }

        public int? OperatorId { get; set; }

        public string? OperatorName { get; set; }

        public OrigoPrivateSubscription? RealOwner { get; set; }
    }
}
