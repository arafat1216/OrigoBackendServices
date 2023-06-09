﻿using CustomerServices.ServiceModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Customer.API.ViewModels
{
    public record User
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }

        public string LastName { get; init; }

        [EmailAddress]
        public string Email { get; init; }
        public DateTime? LastWorkingDay { get; init; } = null;
        public DateTime? LastDayForReportingSalaryDeduction { get; init; } = null;

        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public string MobileNumber { get; init; }

        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public string EmployeeId { get; init; }

        public UserPreference UserPreference { get; init; }

        public string OrganizationName { get; init; }
        public string UserStatusName { get; init; }
        public int UserStatus { get; init; }
        public bool IsActiveState { get; init; }


        public Guid AssignedToDepartment { get; init; }
        public string DepartmentName { get; init; }

        public string Role { get; init; }
        public IList<ManagerOfDTO> ManagerOf { get; init; }

    }
}