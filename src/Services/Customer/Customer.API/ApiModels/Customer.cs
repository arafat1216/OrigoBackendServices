namespace Customer.API.ApiModels
{
    public record Customer
    {
        public System.Guid Id { get; set; }

        public string CompanyName { get; set; }

        public string OrgNumber { get; set; }

        public Address CompanyAddress { get; set; }

        public ContactPerson CustomerContactPerson { get; set; }
    }
}
