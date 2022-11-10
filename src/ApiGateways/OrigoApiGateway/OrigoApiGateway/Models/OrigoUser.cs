using OrigoApiGateway.Models.BackendDTO;
using System.Text.Json.Serialization;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Response object
    /// </summary>
    public record OrigoUser
    {
        public Guid Id { get; init; }

        public string DisplayName { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        [EmailAddress]
        public string Email { get; init; }

        public DateTime? LastWorkingDay { get; init; } = null;

        public DateTime? LastDayForReportingSalaryDeduction { get; init; } = null;

        [Phone]
        public string MobileNumber { get; init; }

        public string EmployeeId { get; init; }

        public string OrganizationName { get; init; }

#nullable enable
        /// <summary>
        ///     The organization ID belonging to the corresponding <see cref="OrganizationName"/>.
        ///     This property is typically only included for cross-customer requests, where the organization details has been requested.
        /// </summary>
        [SwaggerSchema(ReadOnly = true)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Guid? OrganizationId { get; init; }
#nullable restore

        public string Role { get; init; }

        public string UserStatusName { get; init; }

        public int UserStatus { get; init; }

        public bool IsActiveState { get; init; }

        public Guid AssignedToDepartment { get; init; }

        public string DepartmentName { get; init; }

        public UserPreference UserPreference { get; init; }

        public IList<ManagerOf> ManagerOf { get; init; }

    }
}