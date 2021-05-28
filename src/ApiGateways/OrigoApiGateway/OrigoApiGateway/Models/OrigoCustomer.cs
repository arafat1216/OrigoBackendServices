using System;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public record OrigoCustomer
    {
        public OrigoCustomer(CustomerDTO customerDTO){
            Id = customerDTO.Id;
            CompanyName = customerDTO.CompanyName;
            OrgNumber = customerDTO.OrgNumber;
            CompanyAddress = new OrigoAddress(customerDTO.CompanyAddress);
            CustomerContactPerson = new OrigoContactPerson(customerDTO.CustomerContactPerson);
        }

        public Guid Id { get; set; }

        public string CompanyName { get; set; }

        public string OrgNumber { get; set; }

        public OrigoAddress CompanyAddress { get; set; }

        public OrigoContactPerson CustomerContactPerson { get; set; }
    }
}
