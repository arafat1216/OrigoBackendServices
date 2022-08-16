namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public record NewUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string MobileNumber { get; set; }

        public string EmployeeId { get; set; }

        public UserPreference UserPreference { get; set; }

        public string Role { get; set; }
    }
}