using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public class UpdateSubscriptionProduct
    {
        public string Name { get; set; }
        public IList<string> DataPackages { get; set; }
    }
}
