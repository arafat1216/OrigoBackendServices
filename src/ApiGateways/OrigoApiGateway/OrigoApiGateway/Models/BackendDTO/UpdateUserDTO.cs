using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class UpdateUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }
        public string MobilePhone { get; set; }
        public UserPreference UserPreference { get; set; }
        public Guid CallerId { get; set; }
    }
}
