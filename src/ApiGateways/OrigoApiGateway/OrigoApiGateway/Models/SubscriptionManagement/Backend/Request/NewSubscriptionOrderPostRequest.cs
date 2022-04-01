using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement.Backend.Request
{
    public record NewSubscriptionOrderPostRequest
    {
        public int OperatorId { get; set; }

        public int? OperatorAccountId { get; set; }
        public string? OperatorAccountPhoneNumber { get; set; }

        public NewOperatorAccountRequested? NewOperatorAccount { get; set; }

        public int SubscriptionProductId { get; set; }


        public string? DataPackage { get; set; }

        public DateTime OrderExecutionDate { get; set; }

        public IList<string> AddOnProducts { get; set; } = new List<string>();

        public string? SimCardNumber { get; set; }


        public string SimCardAction { get; set; }

        public SimCardAddress? SimCardAddress { get; set; } = null;

        public IList<NewCustomerReferenceValue> CustomerReferenceFields { get; set; } = new List<NewCustomerReferenceValue>();

        public PrivateSubscription? PrivateSubscription { get; set; } = null;
        public BusinessSubscription? BusinessSubscription { get; set; } = null;
        public Guid CallerId { get; set; }
    }
}
