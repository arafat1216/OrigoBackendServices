﻿using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoSubscriptionOrderListItem
    {
        public DateTime CreatedDate { get; set; }
        public string NewSubscriptionOrderOwnerName { get; set; }
        public DateTime TransferDate { get; set; }
    }
}
