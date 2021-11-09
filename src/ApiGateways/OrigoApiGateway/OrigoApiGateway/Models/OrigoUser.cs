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
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            MobileNumber = user.MobileNumber;
            EmployeeId = user.EmployeeId;
            OrganizationName = user.OrganizationName;
            AssignedToDepartments = user.AssignedToDepartments;
            UserPreference = new UserPreference(user.UserPreference);
            IsActive = user.IsActive;
        }

        public Guid Id { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string EmployeeId { get; set; }
        public string OrganizationName { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> AssignedToDepartments { get; set; }
        public UserPreference UserPreference { get; set; }
    }
}