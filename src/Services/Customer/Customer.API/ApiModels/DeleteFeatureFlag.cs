using System;

namespace Customer.API.ApiModels
{
    public class DeleteFeatureFlag
    {
        public string FeatureFlagName { get; set; }
        public Guid? CustomerId { get; set; }
    }
}