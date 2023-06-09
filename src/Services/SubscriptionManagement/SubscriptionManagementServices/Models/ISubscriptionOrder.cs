﻿using Common.Enums;

namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionOrder
    {
        public Guid SubscriptionOrderId { get; set; }
        public DateTime CreatedDate { get;  }
        public SubscriptionOrderTypes OrderType { get;  }
        public string? PhoneNumber { get; }
        public string? SalesforceTicketId { get; set; }
        public string NewSubscriptionOrderOwnerName { get; }
        public DateTime OrderExecutionDate { get; }
        public Guid CreatedBy { get; }
    }
}
