#nullable enable

using Microsoft.AspNetCore.Mvc;

namespace OrigoApiGateway.Models
{
    public class FilterOptionsForHardwareServiceOrder
    {
        [FromQuery(Name = "statusId")]
        public HashSet<int>? StatusIds { get; set; } = null;
    }
}
