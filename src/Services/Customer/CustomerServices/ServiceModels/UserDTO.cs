using System;

namespace CustomerServices.ServiceModels
{
    public record  UserDTO
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string MobileNumber { get; init; }
        public string EmployeeId { get; init; }
        public UserPreferenceDTO UserPreference { get; init; }
        public string OrganizationName { get; init; }
        public bool IsActive { get; init; }
        public string Role { get; set; }
    }
}
