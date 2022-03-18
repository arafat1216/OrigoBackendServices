using System;

namespace Customer.API.ApiModels
{
    public class UpdateUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }
        public string MobileNumber { get; set; }
        public UserPreference UserPreference { get; set; }
        /// <summary>
        /// NB! This Will be removed in a later version
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
