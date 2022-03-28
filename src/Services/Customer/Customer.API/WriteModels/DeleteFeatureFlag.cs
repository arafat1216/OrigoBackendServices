using System;

namespace Customer.API.WriteModels
{
    public class DeleteFeatureFlag
    {
        public string FeatureFlagName { get; set; }
        public Guid? CustomerId { get; set; }
    }
}