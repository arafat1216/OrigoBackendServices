using System;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public record OrigoOperator
    {
        public string OperatorName { get; init; }
        public string Country { get; init; }

        public DateTime CreatedDate { get; init; }
        public Guid CreatedBy { get; init; }

    }
}
