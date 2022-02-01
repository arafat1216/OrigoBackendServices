using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public record OrigoSubscriptionProduct
    {
        public string SubscriptionName { get; set; }
        public string OperatorType { get; set; }
        public IList<string>? DataPackages { get; set; }
    }
}
