using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.ServiceModels
{
    public class UpdateOrganizationDTO
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }

        public string OrganizationNumber { get; set; }
        public int LastDayForReportingSalaryDeduction { get; set; }
        public string PayrollContactEmail { get; set; } = string.Empty;
        public AddressDTO Address { get; set; } = new AddressDTO();

        public ContactPersonDTO ContactPerson { get; set; } = new ContactPersonDTO();

        public Guid? PrimaryLocation { get; set; }
        public Guid? ParentId { get; set; }
        public Guid CallerId { get; set; }
        public bool? AddUsersToOkta { get; set; }
    }
}
