using Common.Seedwork;
using System;

namespace CustomerServices.Models
{
    public class Partner :  Entity
    {
        protected Partner()
        {

        }

        public Partner(Organization organization, Guid callerId)
        {
            ExternalId = Guid.NewGuid();
            OrganizationId = organization.Id;
            Organization = organization;
            CreatedBy = callerId;
            CreatedDate = DateTime.UtcNow;
            LastUpdatedDate = DateTime.UtcNow;
            UpdatedBy = callerId;
        }

        public Guid ExternalId { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
