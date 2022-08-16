using System.Text.Json.Serialization;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record UserAdminDTO
    {
        public Guid UserId { get; init; }

        public string DisplayName { get; init; }

        public string FirstName { get; init; }

        public string LastName { get; init; }

        [EmailAddress]
        public string Email { get; init; }

        [Phone]
        public string MobileNumber { get; init; }

        public string Role { get; init; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IList<Guid> AccessList { get; init; }
    }
}
