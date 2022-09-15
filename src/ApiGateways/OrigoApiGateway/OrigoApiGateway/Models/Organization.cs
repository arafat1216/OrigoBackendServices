namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Response object
    /// </summary>
    public record Organization
    {
        public Guid OrganizationId { get; init; }

        public string Name { get; init; }

        public string OrganizationNumber { get; init; }

        public int LastDayForReportingSalaryDeduction { get; init; } = 1;

        [EmailAddress]
        public string PayrollContactEmail { get; init; } = string.Empty;

        public Address Address { get; init; }

        public OrigoContactPerson ContactPerson { get; init; }

        public NewOrganizationPreferences Preferences { get; init; }

        public Location Location { get; init; }

        public Guid? PartnerId { get; set; }

        public bool? AddUsersToOkta { get; set; }

        public int Status { get; set; }

        public string StatusName { get; set; }
        public string AccountOwner { get; set; }
        public long? TechstepCustomerId { get; set; }
    }
}
