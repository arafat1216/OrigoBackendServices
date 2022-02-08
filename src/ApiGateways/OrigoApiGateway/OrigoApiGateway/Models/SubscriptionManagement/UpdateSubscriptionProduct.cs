using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public class UpdateSubscriptionProduct
    {
        public string SubscriptionName { get; set; }
        public UpdateOperator Operator { get; set; }
        public IList<string> Datapackages { get; set; }
    }
}
