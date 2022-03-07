

namespace SubscriptionManagementServices.ServiceModels
{
    public record class CustomerOperatorAccountDTO
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public int OperatorId { get; set; }
        public Guid OrganizationId { get; set; }
        public string ConnectedOrganizationNumber { get; set; }
    }
}