using System;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public record Organization
    {
        public Organization(OrganizationDTO customerDTO){
            Id = customerDTO.Id;
            CompanyName = customerDTO.CompanyName;
            OrganizationNumber = customerDTO.OrgNumber;
            OrganizationAddress = new Address(customerDTO.CompanyAddress);
            OrganizationContactPerson = new ContactPerson(customerDTO.CustomerContactPerson);
            OrganizationPreferences = customerDTO.OrganizationPreferences;
            OrganizationLocation = customerDTO.OrganizationLocation;
        }

        public Guid Id { get; set; }

        public string CompanyName { get; set; }

        public string OrganizationNumber { get; set; }

        public Address OrganizationAddress { get; set; }

        public ContactPerson OrganizationContactPerson { get; set; }
        public OrganizationPreferences OrganizationPreferences { get; set; }
        public Location OrganizationLocation { get; set; }
    }
}
