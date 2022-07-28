using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record UserDTO
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public DateTime? LastWorkingDay { get; init; } = null;
        public DateTime? LastDayForReportingSalaryDeduction { get; init; } = null;
        public string MobileNumber { get; init; }
        public string EmployeeId { get; init; }
        public string OrganizationName { get; init; }
        public string Role { get; init; }
        public string UserStatusName { get; init; }
        public int UserStatus { get; init; }
        public bool IsActiveState { get; init; }

        public Guid AssignedToDepartment { get; init; }
        public string DepartmentName { get; init; }

        public UserPreferenceDTO UserPreference { get; init; }
        public IList<ManagerOf> ManagerOf { get; init; }
    }
}