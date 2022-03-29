using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoPrivateSubscription
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalPlace { get; set; }
        public string? Country { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? OperatorId { get; set; }
        public string? OperatorName { get; set; }
        public OrigoPrivateSubscription? RealOwner { get; set; }
    }
}
