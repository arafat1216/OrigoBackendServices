using System;

namespace OrigoApiGateway.Models
{
    public class OrigoDeleteFeatureFlag
    {
        public string FeatureFlagName { get; set; }
        public Guid? CustomerId { get; set; }
    }
}