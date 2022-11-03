using Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace OrigoApiGateway.Models
{
    public class FilterOptionsForSubscriptionOrder
    {
        [FromQuery(Name = "orderType")]
        public IList<SubscriptionOrderTypes>? OrderType { get; set; }
    }
}
