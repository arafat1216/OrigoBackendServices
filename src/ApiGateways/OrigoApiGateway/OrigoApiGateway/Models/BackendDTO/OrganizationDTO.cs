using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    /// <summary>
    /// Customer object received from the customer backend services.
    /// </summary>
    public class OrganizationDTO
    {
        public Guid Id { get; set; }

        public string CompanyName { get; set; }

        public string OrgNumber { get; set; }

        public AddressDTO CompanyAddress { get; set; }

        public ContactPersonDTO CustomerContactPerson { get; set; }
        public OrganizationPreferences OrganizationPreferences { get; set; }
        public Location OrganizationLocation { get; set; }
    }
}