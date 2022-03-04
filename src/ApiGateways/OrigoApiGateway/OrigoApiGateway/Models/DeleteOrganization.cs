using System;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public class DeleteOrganization
    {
        public Guid OrganizationId { get; set; }
        public Guid CallerId { get; set; }
        public bool hardDelete { get; set; }
    }
}
