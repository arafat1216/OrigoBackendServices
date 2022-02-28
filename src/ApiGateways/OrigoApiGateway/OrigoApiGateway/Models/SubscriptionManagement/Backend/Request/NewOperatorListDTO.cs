using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement.Backend.Request
{
    public class NewOperatorListDTO
    {
        public List<int> Operators { get; set; }
        public Guid CallerId { get; set; }
    }
}
