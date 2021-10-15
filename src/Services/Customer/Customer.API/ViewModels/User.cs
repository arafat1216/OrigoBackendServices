using System;

namespace Customer.API.ViewModels
{
    public class User
    {
        public User(CustomerServices.Models.User user)
        {
            Id = user.UserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            EmployeeId = user.EmployeeId;
            MobileNumber = user.MobileNumber;
            OrganizationName = user.Customer != null ? user.Customer.OrganizationName : string.Empty;
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string EmployeeId { get; set; }
        public string OrganizationName { get; set; }
    }
}