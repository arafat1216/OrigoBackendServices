namespace SubscriptionManagementServices.ServiceModels
{
    public class NewOperatorAccountRequestedDTO
    {
        public int OperatorId { get; set; }
        public string? OperatorName { get; set; }
        public string? NewOperatorAccountOwner { get; set; }
        public string? OrganizationNumberOwner { get; set; }
        public string? NewOperatorAccountPayer { get; set; }
        public string? OrganizationNumberPayer { get; set; }
    }
}
