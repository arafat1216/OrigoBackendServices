using Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace SubscriptionManagement.API.ViewModels
{
    public class FilterOptionsForSubscriptionOrder
    {
        [FromQuery(Name = "orderType")]
        public IList<SubscriptionOrderTypes>? OrderType { get; set; }
    }
}
