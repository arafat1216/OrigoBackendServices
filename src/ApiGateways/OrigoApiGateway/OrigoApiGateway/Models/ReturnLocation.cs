using System;

namespace OrigoApiGateway.Models
{
    public class ReturnLocation
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string ReturnDescription { get; init; }
        public Guid LocationId { get; init; }
        public Location Location { get; set; }
    }
}
