namespace Customer.API.ApiModels
{
    public class Location
    {
        public Location(CustomerServices.Models.Location organizationLocation)
        {
            Name = organizationLocation.Name;
            Description = organizationLocation.Description;
            Address1 = organizationLocation.Address1;
            Address2 = organizationLocation.Address2;
            PostalCode = organizationLocation.PostalCode;
            City = organizationLocation.City;
            Country = organizationLocation.Country;
        }

        public Location() { }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
