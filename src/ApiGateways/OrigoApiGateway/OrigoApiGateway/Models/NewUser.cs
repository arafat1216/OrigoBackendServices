namespace OrigoApiGateway.Models
{
    public record NewUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string EmployeeId { get; set; }
        public UserPreference UserPreference { get; set; }
    }
}