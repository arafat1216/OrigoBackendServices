using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class NewReturnLocationDTO
    {
        public string Name { get; set; } = string.Empty;
        public string ReturnDescription { get; set; } = string.Empty;
        public Guid LocationId { get; set; }
        public Guid CallerId { get; set; }
    }
}
