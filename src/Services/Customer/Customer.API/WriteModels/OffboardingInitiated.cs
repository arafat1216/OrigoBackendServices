using System;

namespace Customer.API.WriteModels
{
    public record OffboardingInitiated
    {
        public DateTime LastWorkingDay { get; init; }
        public bool BuyoutAllowed { get; set; } = false;
        public Guid CallerId { get; init; }
    }
}
