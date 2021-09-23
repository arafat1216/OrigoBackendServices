using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public record OrigoUser
    {
        public OrigoUser(UserDTO user)
        {
            Id = user.Id;
            DisplayName = user.FirstName + " " + user.LastName;
            Email = user.Email;
            MobileNumber = user.MobileNumber;
            EmployeeId = user.EmployeeId;
            CustomerName = user.CustomerName;
            AssignedToDepartments = user.AssignedToDepartments;
        }

        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerName { get; set; }
        public List<Guid> AssignedToDepartments { get; set; }
    }
}