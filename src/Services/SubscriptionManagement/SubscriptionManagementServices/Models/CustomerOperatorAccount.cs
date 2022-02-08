using Common.Seedwork;


namespace SubscriptionManagementServices.Models
{
    public class CustomerOperatorAccount : Entity
    {
        public CustomerOperatorAccount()
        {

        }
        public CustomerOperatorAccount(Guid organizationId, Guid customerId, string accountNumber, string accountName, int operatorId, Guid callerId)
        {
            OrganizationId = organizationId;
            CustomerId = customerId;
            AccountNumber = accountNumber;
            AccountName = accountName;
            OperatorId = operatorId;
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }

        public Guid OrganizationId { get; set; }
        public Guid CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public virtual Operator Operator { get; set; }
        public int OperatorId { get; set; }
        public virtual ICollection<SubscriptionOrder>? SubscriptionOrders { get; set; }
    }
}
