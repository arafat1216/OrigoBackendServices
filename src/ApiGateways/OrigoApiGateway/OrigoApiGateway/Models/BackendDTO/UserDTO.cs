﻿using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record UserDTO
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string MobileNumber { get; init; }
        public string EmployeeId { get; init; }
        public string OrganizationName { get; init; }
        public string Role { get; init; }
        public string UserStatusName { get; init; }
        public int UserStatus { get; init; }
        public List<Guid> AssignedToDepartments { get; init; }
        public UserPreferenceDTO UserPreference { get; init; }
    }
}