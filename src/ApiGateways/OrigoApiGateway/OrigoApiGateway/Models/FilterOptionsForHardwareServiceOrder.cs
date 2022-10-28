#nullable enable

using Microsoft.AspNetCore.Mvc;

namespace OrigoApiGateway.Models
{
    public class FilterOptionsForHardwareServiceOrder
    {
        [FromQuery]
        public HashSet<int>? StatusIds { get; set; } = null;
    }
}
