using System;

namespace Customer.API.ViewModels
{
    public class NewFeatureFlag
    {
        public string FeatureFlagName { get; set; }
        public Guid? CustomerId { get; set; }
    }
}