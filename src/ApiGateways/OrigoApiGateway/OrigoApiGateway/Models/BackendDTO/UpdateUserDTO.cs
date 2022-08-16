namespace OrigoApiGateway.Models.BackendDTO
{
    public class UpdateUserDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string EmployeeId { get; set; }

        [Phone]
        public string MobileNumber { get; set; }

        public UserPreference UserPreference { get; set; }

        public Guid CallerId { get; set; }
    }
}
