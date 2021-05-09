namespace Customer.API.ViewModels
{
    public record Address
    {
        public string Street { get; set; }

        public string Postcode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}