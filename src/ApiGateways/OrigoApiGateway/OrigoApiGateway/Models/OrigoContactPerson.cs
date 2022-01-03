using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request and response object
    /// </summary>
    public class OrigoContactPerson
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }

        public string Email { get; init; }

        public string PhoneNumber { get; init; }
    }
}