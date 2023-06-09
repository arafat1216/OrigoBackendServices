﻿using System.ComponentModel.DataAnnotations;

namespace CustomerServices.ServiceModels
{
    public record UserDTO
    {
        public Guid Id { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        [EmailAddress]
        public string Email { get; init; }
        public DateTime? LastWorkingDay { get; init; } = null;
        public DateTime? LastDayForReportingSalaryDeduction { get; init; } = null;


        /// <summary>
        ///     The persons phone-number, standardized using the <c>E.164</c> format.
        /// </summary>
        /// <example>+4712345678</example>
        public string MobileNumber { get; init; }

        public string EmployeeId { get; init; }

        public UserPreferenceDTO UserPreference { get; init; }

        public string OrganizationName { get; init; }

#nullable enable
        /// <summary>
        ///     The organization ID belonging to the corresponding <see cref="OrganizationName"/>.
        /// </summary>
        public Guid? OrganizationId { get; init; }
#nullable restore

        public string UserStatusName { get; init; }
        public int UserStatus { get; init; }
        public bool IsActiveState { get; init; }

        public Guid AssignedToDepartment { get; init; }
        public string DepartmentName { get; set; }

        public string Role { get; set; }
        public IList<ManagerOfDTO> ManagerOf { get; set; }

    }
}
