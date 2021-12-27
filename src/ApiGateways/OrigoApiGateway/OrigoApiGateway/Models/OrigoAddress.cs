using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Models
{
    public class OrigoAddress
    {
        public string Street { get; init; }

        public string Postcode { get; init; }

        public string City { get; init; }

        public string Country { get; init; }
    }
}