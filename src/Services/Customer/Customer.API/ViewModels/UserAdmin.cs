using System;
using System.Collections.Generic;

#nullable enable

namespace Customer.API.ViewModels
{
    public class UserAdmin
    {
        public UserAdmin(
            Guid userId,
            string firstName,
            string lastName,
            string email,
            string mobileNumber,
            string role,
            IReadOnlyCollection<Guid> accessList)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            DisplayName = firstName + " " + lastName;
            Email = email;
            MobileNumber = mobileNumber;
            Role = role;
            AccessList = accessList;
        }
        public Guid UserId { get; }
        public string DisplayName { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string MobileNumber { get; }
        public string Role { get; }

        public IReadOnlyCollection<Guid> AccessList { get; }
    }
}
