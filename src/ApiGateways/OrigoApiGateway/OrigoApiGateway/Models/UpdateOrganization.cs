using System;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public record UpdateOrganization
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string OrganizationNumber { get; set; }
        public int LastDayForReportingSalaryDeduction { get; set; }
        [RegularExpression("^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}$", ErrorMessage = "Invalid Email address!!")]
        public string PayrollContactEmail { get; set; } = string.Empty;
        public Address Address { get; set; }
        public OrigoContactPerson ContactPerson { get; set; }
        public Guid? PrimaryLocation { get; set; }
        public Guid? ParentId { get; set; }

        public bool? AddUsersToOkta { get; set; }
    }
}
