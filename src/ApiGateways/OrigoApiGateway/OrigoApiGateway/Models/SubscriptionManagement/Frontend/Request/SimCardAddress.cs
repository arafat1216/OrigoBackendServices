﻿namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class SimCardAddress
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalPlace { get; set; }
        [MaxLength(2)]
        public string? Country { get; set; }
    }
}
