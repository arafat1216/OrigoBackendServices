using System;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public record UpdateOrganization
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string OrganizationNumber { get; set; }
        public Address Address { get; set; }
        public OrigoContactPerson ContactPerson { get; set; }
        public Guid? PrimaryLocation { get; set; }
        public Guid? ParentId { get; set; }
    }
}
