using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record UserDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerName { get; set; }
    }
}