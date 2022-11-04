using Common.Enums;
using Microsoft.AspNetCore.Mvc;

namespace SubscriptionManagement.API.ViewModels
{
    public class FilterOptionsForSubscriptionOrder
    {
        [FromQuery(Name = "orderTypeId")]
        public IList<int>? OrderTypes { get; set; }
    }
}
