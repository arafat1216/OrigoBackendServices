using System;
using System.ComponentModel.DataAnnotations;

namespace Customer.API.ViewModels
{
    public record NewUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public UserPreference UserPreference { get; set; }

        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public string EmployeeId { get; set; }

        public Guid CallerId { get; set; }

        public string Role { get; set; }
    }
}