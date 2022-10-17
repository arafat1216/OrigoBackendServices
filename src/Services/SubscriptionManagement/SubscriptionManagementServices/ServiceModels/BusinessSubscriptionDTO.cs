using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.ServiceModels
{
    public class BusinessSubscriptionDTO
    {
        public string? Name { get; set; }
        public string? OrganizationNumber { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalPlace { get; set; }
        [MaxLength(2)]
        public string? Country { get; set; }
        public string? OperatorName { get; set; }
        public string? ContactPerson { get; set; }
    }
}
