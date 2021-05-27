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
            CustomerName = user.Customer.CompanyName;
            EmployeeId = user.EmployeeId;
            MobileNumber = user.MobileNumber;
            CustomerName = user.Customer != null ? user.Customer.CompanyName : string.Empty;
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string EmployeeId { get; set; }
        public string CustomerName { get; set; }
    }
}