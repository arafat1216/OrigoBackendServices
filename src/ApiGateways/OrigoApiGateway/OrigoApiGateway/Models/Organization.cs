using System;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public record Organization
    {
        public Organization(OrganizationDTO customerDTO){
            OrganizationId = customerDTO.OrganizationId;
            OrganizationName = customerDTO.OrganizationName;
            OrganizationNumber = customerDTO.OrganizationNumber;
            OrganizationAddress = new Address(customerDTO.OrganizationAddress);
            OrganizationContactPerson = new ContactPerson(customerDTO.OrganizationContactPerson);
            OrganizationPreferences = customerDTO.OrganizationPreferences;
            OrganizationLocation = customerDTO.OrganizationLocation;
        }

        public Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationNumber { get; set; }

        public Address OrganizationAddress { get; set; }

        public ContactPerson OrganizationContactPerson { get; set; }
        public NewOrganizationPreferences OrganizationPreferences { get; set; }
        public NewLocation OrganizationLocation { get; set; }
    }
}
