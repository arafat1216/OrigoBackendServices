using System;

namespace Customer.API.WriteModels
{
    public class NewFeatureFlag
    {
        public string FeatureFlagName { get; set; }
        public Guid? CustomerId { get; set; }
    }
}