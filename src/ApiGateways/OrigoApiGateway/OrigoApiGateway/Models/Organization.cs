using System;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public record Organization
    {
        public Organization(OrganizationDTO customerDTO){
            Id = customerDTO.Id;
            CompanyName = customerDTO.OrganizationName;
            OrganizationNumber = customerDTO.OrganizationNumber;
            OrganizationAddress = new Address(customerDTO.OrganizationAddress);
            OrganizationContactPerson = new ContactPerson(customerDTO.OrganizationContactPerson);
            OrganizationPreferences = customerDTO.OrganizationPreferences;
            OrganizationLocation = customerDTO.OrganizationLocation;
        }

        public Guid Id { get; set; }

        public string CompanyName { get; set; }

        public string OrganizationNumber { get; set; }

        public Address OrganizationAddress { get; set; }

        public ContactPerson OrganizationContactPerson { get; set; }
        public NewOrganizationPreferences OrganizationPreferences { get; set; }
        public NewLocation OrganizationLocation { get; set; }
    }
}
