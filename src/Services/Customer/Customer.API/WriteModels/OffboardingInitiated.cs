using System;

namespace Customer.API.WriteModels
{
    public record OffboardingInitiated
    {
        public DateTime LastWorkingDay { get; init; }
        public Guid CallerId { get; init; }
    }
}
