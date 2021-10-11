using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.API.ViewModels
{
    public record DeleteOrganization
    {
        public Guid OrganizationId { get; set; }
        public Guid CallerId { get; set; }
        public bool HardDelete { get; set; }
    }
}
