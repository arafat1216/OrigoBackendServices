namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request and response object
    /// </summary>
    public class OrigoContactPerson
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }

        [EmailAddress]
        [MaxLength(320)]
        public string Email { get; init; }

        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; init; }
    }
}