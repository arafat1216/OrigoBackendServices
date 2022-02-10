using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class PartnerDto
    {
        public Guid ExternalId { get; set; }
        //public Guid OrganizationId { get; set; }
        public OrganizationDTO Organization { get; set; }
    }
}
