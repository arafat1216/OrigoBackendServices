using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class Partner
    {
        public Guid ExternalId { get; set; }
        public Organization Organization { get; set; }
    }
}
