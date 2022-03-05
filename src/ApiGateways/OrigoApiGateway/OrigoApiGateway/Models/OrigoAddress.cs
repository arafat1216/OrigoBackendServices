namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request and response object
    /// </summary>
    public class OrigoAddress
    {
        public string Street { get; init; }

        public string Postcode { get; init; }

        public string City { get; init; }

        public string Country { get; init; }
    }
}