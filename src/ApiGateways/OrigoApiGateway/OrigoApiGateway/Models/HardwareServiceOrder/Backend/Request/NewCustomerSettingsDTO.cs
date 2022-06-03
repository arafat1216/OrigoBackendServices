using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request
{
    public class NewCustomerSettingsDTO
    {
        public Guid CustomerId { get; set; }
        public string ServiceId { get; set; }
        public LoanDevice LoanDevice { get; set; }
        public int ProviderId { get; set; }
        public List<int> AssetCategoryIds { get; set; }
    }
}
