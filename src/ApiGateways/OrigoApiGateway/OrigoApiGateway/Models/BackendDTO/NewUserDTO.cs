﻿namespace OrigoApiGateway.Models.BackendDTO
{
    public record NewUserDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string MobileNumber { get; set; }

        public string EmployeeId { get; set; }

        public UserPreference UserPreference { get; set; }

        public Guid CallerId { get; set; }

        public string Role { get; set; }
        /// <summary>
        /// If this user can only be activated if the customer is activated
        /// </summary>
        public bool NeedsOnboarding { get; set; } = false;
    }
}
