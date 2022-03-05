using System;

namespace OrigoApiGateway.Models
{
    public class Partner
    {
        public Guid ExternalId { get; set; }
        public Organization Organization { get; set; }
    }
}
