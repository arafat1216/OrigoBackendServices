using Customer.API.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace Customer.API.WriteModels
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

        /// <summary>
        /// If this user can only be activated if the customer is activated
        /// </summary>
        public bool NeedsOnboarding { get; set; } = false;

        /// <summary>
        /// Don't add the user to Okta even though the customer has AddToOkta
        /// </summary>
        public bool SkipAddingUserToOkta { get; set; } = false;
    }
}