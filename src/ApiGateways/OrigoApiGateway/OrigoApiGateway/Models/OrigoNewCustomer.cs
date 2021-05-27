namespace OrigoApiGateway.Models
{
    public record OrigoNewCustomer
    {
        public string CompanyName { get; set; }

        public string OrgNumber { get; set; }

        public OrigoAddress CompanyAddress { get; set; }

        public OrigoContactPerson CustomerContactPerson { get; set; }
    }
}
