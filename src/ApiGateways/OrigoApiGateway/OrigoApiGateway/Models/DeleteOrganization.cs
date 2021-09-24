using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class DeleteOrganization
    {
        public Guid OrganizationId { get; set; }
        public Guid CallerId { get; set; }
        public bool hardDelete { get; set; }
    }
}
