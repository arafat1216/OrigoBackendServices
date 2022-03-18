using System;

namespace Customer.API.ApiModels
{
    public class NewFeatureFlag
    {
        public string FeatureFlagName { get; set; }
        public Guid? CustomerId { get; set; }
    }
}