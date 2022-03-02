namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionOrder
    {
        public DateTime CreatedDate { get;  }
        public string OrderType { get;  }
        public string PhoneNumber { get; }
        public string NewSubscriptionOrderOwnerName { get; }
        public DateTime TransferDate { get; }
        public Guid CreatedBy { get; }

    }
}
