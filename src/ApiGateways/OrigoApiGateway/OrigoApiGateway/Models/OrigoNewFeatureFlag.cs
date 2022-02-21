using System;

namespace OrigoApiGateway.Models
{
    public class OrigoNewFeatureFlag
    {
        public string FeatureFlagName { get; set; }
        public Guid? CustomerId { get; set; }
    }
}