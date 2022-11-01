#nullable enable

using Microsoft.AspNetCore.Mvc;

namespace OrigoApiGateway.Models
{
    public class FilterOptionsForHardwareServiceOrder
    {
        [FromQuery]
        public HashSet<int>? StatusId { get; set; } = null;
    }
}
