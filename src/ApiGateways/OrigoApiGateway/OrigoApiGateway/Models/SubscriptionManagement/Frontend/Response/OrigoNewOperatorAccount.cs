namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoNewOperatorAccount
    {
        public int OperatorId { get; set; }
        public string? OperatorName { get; set; }
        public string? NewOperatorAccountOwner { get; set; }
        public string? OrganizationNumberOwner { get; set; }
        public string? NewOperatorAccountPayer { get; set; }
        public string? OrganizationNumberPayer { get; set; }
    }
}
