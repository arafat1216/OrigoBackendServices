﻿namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public record OrigoOperator
    {
        public string Name { get; init; }
        public string Country { get; init; }
        public int Id { get; set; }
    }
}
