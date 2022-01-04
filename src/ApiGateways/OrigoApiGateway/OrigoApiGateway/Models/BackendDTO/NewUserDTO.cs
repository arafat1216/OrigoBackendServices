using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record NewUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string EmployeeId { get; set; }
        public UserPreference UserPreference { get; set; }
        public Guid CallerId { get; set; }
        public string Role { get; set; }
    }
}
