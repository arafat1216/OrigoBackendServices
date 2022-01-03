using System;
using System.Collections.Generic;

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
        public string Email { get; init; }
        public string MobileNumber { get; init; }
        public string EmployeeId { get; init; }
        public string OrganizationName { get; init; }
        public string Role { get; init; }
        public bool IsActive { get; init; }
        public List<Guid> AssignedToDepartments { get; init; }
        public UserPreference UserPreference { get; init; }
    }
}