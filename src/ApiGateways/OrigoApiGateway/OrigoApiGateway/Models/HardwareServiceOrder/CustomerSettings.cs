using System;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder
{
    public class CustomerSettings
    {
        public Guid CustomerId { get; set; }
        public string ServiceId { get; set; }
        public LoanDevice LoanDevice { get; set; }
    }
}
