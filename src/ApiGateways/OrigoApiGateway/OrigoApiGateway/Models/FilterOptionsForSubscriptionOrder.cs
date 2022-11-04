using Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace OrigoApiGateway.Models
{
    public class FilterOptionsForSubscriptionOrder
    {
        [FromQuery(Name = "orderTypeId")]
        public IList<int>? OrderTypes { get; set; }
    }
}
