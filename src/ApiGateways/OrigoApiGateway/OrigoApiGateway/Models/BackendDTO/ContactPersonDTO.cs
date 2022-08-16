namespace OrigoApiGateway.Models.BackendDTO
{
    public class ContactPersonDTO
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }

        [EmailAddress]
        public string Email { get; init; }

        [Phone]
        public string PhoneNumber { get; init; }
    }
}