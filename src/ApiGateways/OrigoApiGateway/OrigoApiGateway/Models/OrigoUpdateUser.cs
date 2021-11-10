namespace OrigoApiGateway.Models
{
    public class OrigoUpdateUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }
        public UserPreference UserPreference { get; set; }
    }
}
