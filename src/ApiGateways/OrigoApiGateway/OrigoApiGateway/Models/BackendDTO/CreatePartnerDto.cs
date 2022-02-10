using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record CreatePartnerDto
    {
        public Guid OrganizationId { get; set; }
        public Guid CallerId { get; set; }
    }
}
