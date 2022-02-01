namespace SubscriptionManagement.API.ViewModels
{
    public class CustomerOperatorAccount
    {
        public CustomerOperatorAccount(SubscriptionManagementServices.Models.CustomerOperatorAccount customerOperatorAccount)
        {
            OrganizationId = customerOperatorAccount.OrganizationId;
            AccountNumber = customerOperatorAccount.AccountNumber;
            AccountName = customerOperatorAccount.AccountName;
            OrganizationId = customerOperatorAccount.OrganizationId;
        }

        public Guid OrganizationId { get; set; }
        public string AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public int OperatorId { get; set; }
    }
}
