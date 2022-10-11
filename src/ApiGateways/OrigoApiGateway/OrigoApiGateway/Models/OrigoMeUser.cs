using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models;

/// <summary>
///     Response model for ME endpoint
/// </summary>
public record OrigoMeUser
{
    public Guid Id { get; init; }

    public string DisplayName { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    [EmailAddress] public string Email { get; init; }

    public DateTime? LastWorkingDay { get; init; } = null;

    public DateTime? LastDayForReportingSalaryDeduction { get; init; } = null;

    [Phone] public string MobileNumber { get; init; }

    public string EmployeeId { get; init; }

    public string OrganizationName { get; init; }

    public string Role { get; init; }

    public string UserStatusName { get; init; }

    public int UserStatus { get; init; }

    public bool IsActiveState { get; init; }

    public Guid AssignedToDepartment { get; init; }

    public string DepartmentName { get; init; }

    public UserPreference UserPreference { get; init; }

    public IList<ManagerOf> ManagerOf { get; init; }

    public List<string> PermissionNames { get; init; } = new();

    public List<string> AccessList { get; init; } = new();
}