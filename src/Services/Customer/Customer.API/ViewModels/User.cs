using System;
using System.ComponentModel.DataAnnotations;

namespace Customer.API.ViewModels
{
    public record User
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; }

        public string LastName { get; init; }

        [EmailAddress]
        public string Email { get; init; }

        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public string MobileNumber { get; init; }

        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public string EmployeeId { get; init; }

        public UserPreference UserPreference { get; init; }

        public string OrganizationName { get; init; }
        public string UserStatusName { get; init; }
        public int UserStatus { get; init; }


        public string Role { get; init; }
    }
}