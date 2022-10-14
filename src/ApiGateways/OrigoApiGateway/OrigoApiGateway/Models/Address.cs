namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request and response object
    /// </summary>
    public class Address
    {
        public string Street { get; init; }

        public string Postcode { get; init; }

        public string City { get; init; }
        [MaxLength(2)]
        public string Country { get; init; }
    }
}