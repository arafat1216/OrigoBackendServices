using OrigoApiGateway.Models.BackendDTO;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public record OrigoSubscriptionProduct
    {
        public string SubscriptionName { get; set; }
        public OperatorDTO OperatorType { get; set; }
        public IList<string>? DataPackages { get; set; }
    }
}
