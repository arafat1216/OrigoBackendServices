namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionOrder
    {
        public Guid SubscriptionOrderId { get; set; }
        public DateTime CreatedDate { get;  }
        public string OrderType { get;  }
        public string PhoneNumber { get; }
        public string SalesforceTicketId { get; set; }
        public string NewSubscriptionOrderOwnerName { get; }
        public DateTime TransferDate { get; }
        public Guid CreatedBy { get; }
    }
}
