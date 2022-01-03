using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    /// <summary>
    /// Customer object received from the customer backend services.
    /// </summary>
    public class OrganizationDTO
    {
        public Guid OrganizationId { get; set; }

        public string Name { get; set; }

        public string OrganizationNumber { get; set; }

        public AddressDTO Address { get; set; }

        public ContactPersonDTO ContactPerson { get; set; }
        public OrganizationPreferencesDTO Preferences { get; set; }
        public LocationDTO Location { get; set; }
    }
}