using Customer.API.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace Customer.API.WriteModels
{
    public class UpdateUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        [MaxLength(320)]
        public string Email { get; set; }

        public string EmployeeId { get; set; }
        [Phone]
        [MaxLength(15)]
        public string MobileNumber { get; set; }

        public UserPreference UserPreference { get; set; }

        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
