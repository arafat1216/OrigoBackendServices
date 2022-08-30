
namespace SubscriptionManagementServices.ServiceModels
{
    public class BusinessSubscriptionResponse
    {
       public string? Name { get; set; }
        public string? OrganizationNumber { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalPlace { get; set; }
        public string? Country { get; set; }
        public int? OperatorId { get; set; }
        public string? OperatorName { get; set; }
        public string? ContactPerson { get; set; }
    }
}
