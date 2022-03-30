using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public record OrigoNewSubscriptionOrder
    {
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }

        public int? OperatorAccountId { get; set; }
        public string? OperatorAccountPhoneNumber { get; set; }

        public OrigoNewOperatorAccount? NewOperatorAccount { get; set; }

        public string SubscriptionProductName { get; set; }


        public string? DataPackage { get; set; }

        public DateTime OrderExecutionDate { get; set; }

        public IList<string> AddOnProducts { get; set; } = new List<string>();

        public string? SimCardNumber { get; set; }


        public string SimCardAction { get; set; }

        public SimCardAddress SimCardAddress { get; set; } = null;

        public IList<NewCustomerReferenceValue> CustomerReferenceFields { get; set; } = new List<NewCustomerReferenceValue>();

        public OrigoPrivateSubscription? PrivateSubscription { get; set; } = null;
        public OrigoBusinessSubscription? BusinessSubscription { get; set; } = null;
    }
}
