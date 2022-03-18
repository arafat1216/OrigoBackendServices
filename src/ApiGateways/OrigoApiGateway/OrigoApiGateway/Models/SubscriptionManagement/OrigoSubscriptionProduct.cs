using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public record OrigoSubscriptionProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OperatorId { get; set; }
        public IList<string>? DataPackages { get; set; }
        public bool IsGlobal { get; set; }
    }
}
