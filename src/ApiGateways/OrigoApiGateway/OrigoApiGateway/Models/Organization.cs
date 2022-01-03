using System;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Response object
    /// </summary>
    public record Organization
    {
        public Guid OrganizationId { get; init; }
        public string Name { get; init; }
        public string OrganizationNumber { get; init; }
        public OrigoAddress Address { get; init; }
        public OrigoContactPerson ContactPerson { get; init; }
        public NewOrganizationPreferences Preferences { get; init; }
        public NewLocation Location { get; init; }
    }
}
