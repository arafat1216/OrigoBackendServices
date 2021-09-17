using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public record UpdateOrganization
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }

        public string OrganizationNumber { get; set; }

        public Address OrganizationAddress { get; set; }

        public ContactPerson OrganizationContactPerson { get; set; }

        public NewLocation OrganizationLocation { get; set; }

        public Guid PrimaryLocation { get; set; }
        public Guid ParentId { get; set; }
        public Guid CallerId { get; set; }

        public string ContactEmail { get; set; }

        public string InternalNotes { get; set; }

        public NewOrganizationPreferences OrganizationPreferences { get; set; }
    }
}
