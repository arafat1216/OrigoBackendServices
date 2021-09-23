using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    /// <summary>
    /// Customer object received from the customer backend services.
    /// </summary>
    public class OrganizationDTO
    {
        public Guid Id { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationNumber { get; set; }

        public AddressDTO OrganizationAddress { get; set; }

        public ContactPersonDTO OrganizationContactPerson { get; set; }
        public NewOrganizationPreferences OrganizationPreferences { get; set; }
        public NewLocation OrganizationLocation { get; set; }
    }
}