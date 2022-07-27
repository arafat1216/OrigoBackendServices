using CustomerServices.ServiceModels;
using System;

namespace Customer.API.WriteModels
{
    public record UpdateOrganization
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }

        public string OrganizationNumber { get; set; }
        public int LastDayForReportingSalaryDeduction { get; set; }
        public string PayrollContactEmail { get; set; } = string.Empty;
        public AddressDTO Address { get; set; }

        public ContactPersonDTO ContactPerson { get; set; }

        public Guid? PrimaryLocation { get; set; }
        public Guid? ParentId { get; set; }
        public Guid CallerId { get; set; }
        public bool? AddUsersToOkta { get; set; }
    }
}
