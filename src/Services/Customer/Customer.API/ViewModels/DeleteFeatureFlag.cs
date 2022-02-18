using System;

namespace Customer.API.ViewModels
{
    public class DeleteFeatureFlag
    {
        public string FeatureFlagName { get; set; }
        public Guid? CustomerId { get; set; }
    }
}