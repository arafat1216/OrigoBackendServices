using System;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public record Organization
    {
        public Organization(OrganizationDTO customerDTO){
            OrganizationId = customerDTO.OrganizationId;
            Name = customerDTO.Name;
            OrganizationNumber = customerDTO.OrganizationNumber;
            Address = new Address(customerDTO.Address);
            ContactPerson = new ContactPerson(customerDTO.ContactPerson);
            Preferences = customerDTO.Preferences;
            Location = customerDTO.Location;
        }

        public Guid OrganizationId { get; set; }

        public string Name { get; set; }

        public string OrganizationNumber { get; set; }

        public Address Address { get; set; }

        public ContactPerson ContactPerson { get; set; }
        public NewOrganizationPreferences Preferences { get; set; }
        public NewLocation Location { get; set; }
    }
}
