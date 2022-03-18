using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public record OrigoSubscriptionProduct
    {
        public int Id { get; set; }
        public string SubscriptionName { get; set; }
        public int OperatorId { get; set; }
        public IList<string>? Datapackages { get; set; }
        public bool IsGlobal { get; set; }
    }
}
